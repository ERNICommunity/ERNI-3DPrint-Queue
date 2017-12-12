namespace ERNI.Q3D
{
    public class MaintenanceProvider : IMaintenanceProvider
    {
        private readonly bool _isUnderMaintenance;

        public MaintenanceProvider(bool isUnderMaintenance)
        {
            _isUnderMaintenance = isUnderMaintenance;
        }

        public bool IsUnderMaintenance => _isUnderMaintenance;
    }
}