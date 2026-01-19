namespace SSS.Quality1500.Presentation.Interfaces;

using SSS.Quality1500.Presentation.Models;
using System.Collections.ObjectModel;
using System.Data;


/// <summary>
/// Interface para el servicio de configuración de estado de UI
/// Gestiona el estado compartido entre ViewModels durante la sesión
/// Específico de la capa de Presentación
/// </summary>
public interface IUiConfigurationService {
        #region File Configuration

        string VkFile { get; set; }
        string DatFile { get; set; }
        string IdxFile { get; set; }
        string ImgFileIn { get; set; }
        string ImgFileOut { get; set; }
        string PathOut { get; set; }

        #endregion

        #region Processing Options

        bool Is837StampingChecked { get; set; }
        bool IsIdaChecked { get; set; }
        bool IsZipperChecked { get; set; }
        bool IsConduceChecked { get; set; }
        bool IsBackupChecked { get; set; }
        bool IsConduceDummyChecked { get; set; }
        bool IsVersionChecked { get; set; }
        bool IsRemoveHeaderChecked { get; set; }
        bool Is837ToXlsChecked { get; set; }

        #endregion

        #region Batch Information - UI Specific (ObservableCollection)

        ObservableCollection<string> BatchItems { get; }
        ObservableCollection<VkFileRecord> SelectedBatches { get; }
        DateTime SelectedDate { get; set; }
        string TotalImagenes { get; set; }
        string TotalDocumentos { get; set; }

        #endregion

        #region VdeTable
        public DataTable VdeTable { get; set; }
        #endregion

        #region Events

        event Action? ConfigurationChanged;

        #endregion

        #region Methods

        void UpdateConfiguration(
            string vkFile, string datFile, string idxFile, string imgFileIn, string imgFileOut,string pathOut,
            bool is837Stamping, bool isIda, bool isZipper, bool isConduce, bool isBackup,
            bool isConduceDummy, bool isVersion, bool isRemoveHeader, bool is837ToXls,
            DateTime selectedDate, string totalImagenes, string totalDocumentos, DataTable vdeTable);

        void ClearBatches();
        void ClearAllConfiguration();
        bool IsConfigurationComplete();

        #endregion
    }
