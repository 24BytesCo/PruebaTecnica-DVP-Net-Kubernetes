namespace PruebaTecnica_DVP_Net_Kubernetes.Dtos.User
{
    public class UserReducerResponseDto
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RoleName { get; set; }
        public string? RoleId { get; set; }
        public string? RoleCode { get; set; }
    }
}
