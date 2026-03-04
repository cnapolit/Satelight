namespace Server.Models.Database;

public class Operation : DatabaseObject
{
    public Guid           HostOpId { get; set; }
    public OperationState State    { get; set; }
    public OperationType  Type     { get; set; }
    public short          Progress { get; set; }
    public Guid           HostId   { get; set; }
    public Guid           TargetId { get; set; }

    public Host Host { get; set; }
}