namespace YallaFit.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActivePrograms { get; set; }
        public int CompletedSessions { get; set; }
        public decimal UserGrowth { get; set; }
        public decimal ProgramGrowth { get; set; }
        public decimal SessionGrowth { get; set; }
    }

    public class ProgramStatsDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ActiveUsers { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal GrowthRate { get; set; }
        public List<int> ChartData { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class DashboardDataDto
    {
        public DashboardStatsDto Stats { get; set; } = new();
        public List<ProgramStatsDto> TopPrograms { get; set; } = new();
        public List<RecentActivityDto> RecentActivity { get; set; } = new();
    }
}
