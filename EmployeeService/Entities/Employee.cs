using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

[DataContract]
public class Employee
{
    [Key]
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int? ManagerID { get; set; }
    [DataMember]
    public bool Enable { get; set; }
    [DataMember]
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public Employee()
    { }
}