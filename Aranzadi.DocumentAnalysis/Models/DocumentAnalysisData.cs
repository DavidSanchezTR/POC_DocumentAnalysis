
using Aranzadi.DocumentAnalysis.Models;

namespace Aranzadi.DocumentAnalysis.Models
{	
	public class Error
	{
		public string? Reason;
		public string? Message { get; set; }
		public string? Link { get; set; }
	}

	public class ExecutionStatus
	{
		public string? State { get; set; }
		public List<Error>? Errors { get; set; }
	}

	public class TaskParameter
	{
		public string? Country { get; set; }

		public string? NotificationType { get; set; }
	}

	
	public class AnalysisJobRequest
	{
		public string? Guid { get; set; }
		public string? Name { get; set; }
		public string? ResourceType { get; set; }	
		public List<JobTask>? Tasks { get; set; }
		public List<SourceDocument>? SourceDocuments { get; set; }
		public ExecutionStatus? ExecutionStatus { get; set; }
	}

	public class SourceDocument
	{
		public string? ClientDocumentId { get; set; }
		public string? AzureBlobUri { get; set; }
		public string? DocType { get; set; }
		public string? Format { get; set; }
		public string? CollectionId { get; set; }
		public ExecutionStatus? ExecutionStatus { get; set; }
		public string? Uri { get; set; }
		public object?  Statistics { get; set; }
	}

	public class JobTask
	{		
		public string? Type { get; set; }
		public TaskParameter? Parameters { get; set; }
	}

	public class JobLink
	{
		public string? Rel { get; set; }
		public string? Href { get; set; }
		public string? Action { get; set; }
		public List<string>? Types { get; set; }
	}

	public class JobResponseParameter
	{
	}

	public class JobResponseTask
	{
		public string? Type { get; set; }
		public JobResponseParameter? Parameters { get; set; }
	}

	public class AnalysisJobResponse
	{
		public string Guid { get; set; }
		public string Name { get; set; }
		public List<SourceDocument> SourceDocuments { get; set; }
		public ExecutionStatus ExecutionStatus { get; set; }
		public List<JobLink> Links { get; set; }
		public string ResourceType { get; set; }
		public object Features { get; set; }
		public List<JobResponseTask> Tasks { get; set; }
	}




}
