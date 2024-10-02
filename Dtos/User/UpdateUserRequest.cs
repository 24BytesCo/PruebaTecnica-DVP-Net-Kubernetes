namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.User
{
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
    }
}
