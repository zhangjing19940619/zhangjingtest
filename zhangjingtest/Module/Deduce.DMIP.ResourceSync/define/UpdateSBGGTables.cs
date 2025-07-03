namespace Deduce.DMIP.ResourceSync
{
    public class UpdateSBGGTables
    {
        public UpdateSBGGTables(bool saveSBGGB, bool saveLBB, bool saveSync)
        {
            _saveSBGGBSuccess = saveSBGGB;
            _saveLBBSuccess = saveLBB;
            _saveSyncSuccess = saveSync;
        }
        bool _saveSBGGBSuccess;
        bool _saveLBBSuccess;
        bool _saveSyncSuccess;
        public bool SaveSBGGBSuccess
        {
            get { return _saveSBGGBSuccess; }
            set { _saveSBGGBSuccess = value; }
        }
        public bool SaveLBBSuccess
        {
            get { return _saveLBBSuccess; }
            set { _saveLBBSuccess = value; }
        }
        public bool SaveSyncSuccess
        {
            get { return _saveSyncSuccess; }
            set { _saveSyncSuccess = value; }
        }
    }
}
