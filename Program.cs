using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System.Security.Principal;


public class Program
{
    static void Main(string[] args)
    {
        //Add the following to the Main method:
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=lukacosmosdbaccount;AccountKey=nS8zKyHWZOZu0XtfntN2O0ee8mGmiE0zwE4mXiytHfJ8YmBm6A46I0f8PpwMkQh5FhIwvCrX7RDNACDbxmdCkA==;TableEndpoint=https://lukacosmosdbaccount.table.cosmos.azure.com:443/;";

        var account = CloudStorageAccount.Parse(connectionString);

        var client = account.CreateCloudTableClient();

        var projectTable = client.GetTableReference("Project");
        var taskTable = client.GetTableReference("Task");

        projectTable.CreateIfNotExistsAsync().Wait();
        taskTable.CreateIfNotExistsAsync().Wait();

        ProjectEntity projectEntity1 = new ProjectEntity("1")
        {
            Name = "Translator",
            Code = "TRN"
        };

        ProjectEntity projectEntity2 = new ProjectEntity("2")
        {
            Name = "Serializer",
            Code = "SER"
        };

        ProjectEntity projectEntity3 = new ProjectEntity("3")
        {
            Name = "Deserializer",
            Code = "DER"
        };

        TaskEntity taskEntity1 = new TaskEntity("1")
        {
            TaskName = "TranslatorTask",
            TaskDescription = "Translating file to JSON",
            ProjectID = projectEntity1.PartitionKey
        };

        TaskEntity taskEntity2 = new TaskEntity("2")
        {
            TaskName = "SerializerTask",
            TaskDescription = "Serializing data to JSON",
            ProjectID = projectEntity2.PartitionKey
        };

        TaskEntity taskEntity3 = new TaskEntity("3")
        {
            TaskName = "DeserializerTask",
            TaskDescription = "Deserializing data from JSON",
            ProjectID = projectEntity3.PartitionKey
        };

        var listOfProjects = new List<ProjectEntity> { projectEntity1, projectEntity2, projectEntity3 };
        var listOfTasks = new List<TaskEntity> { taskEntity1, taskEntity2, taskEntity3 };

        foreach (var project in listOfProjects)
        {
            TableOperation insertOperation = TableOperation.Insert(project);
            projectTable.ExecuteAsync(insertOperation).Wait();
            Console.WriteLine(project.RowKey + $" inserted into {projectTable.Name} table");
        }

        foreach (var task in listOfTasks)
        {
            TableOperation insertOperation = TableOperation.Insert(task);
            taskTable.ExecuteAsync(insertOperation).Wait();
            Console.WriteLine(task.RowKey + $" inserted into {taskTable.Name} table");
        }

    }
}

public class ProjectEntity : TableEntity
{
    public ProjectEntity(string projectId)
    {
        this.PartitionKey = "Project"; this.RowKey = projectId;
    }
    public ProjectEntity() { }
    public string Name { get; set; }
    public string Code { get; set; }
}

public class TaskEntity : TableEntity
{
    public TaskEntity(string taskId)
    {
        this.PartitionKey = "Task"; this.RowKey = taskId;
    }
    public TaskEntity() { }
    // To add additional fields to Task entity add below public string fields lines, ex public string *field name*
    public string TaskName { get; set; }
    public string TaskDescription { get; set; }
    public string ProjectID { get; set; }
}