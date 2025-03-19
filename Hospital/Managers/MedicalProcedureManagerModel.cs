using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    class MedicalProcedureManagerModel
    {
        public static ObservableCollection<Procedure> s_procedureList { get; private set; }
        private readonly MedicalProceduresDatabaseService _medicalProcedureDBService;

        public MedicalProcedureManagerModel(MedicalProceduresDatabaseService dbService)
        {
            _medicalProcedureDBService = dbService;
            s_procedureList = new ObservableCollection<Procedure>();
        }

        // This method will be used to get the procedures from the in memory repository
        public ObservableCollection<Procedure> GetProcedures()
        {
            return s_procedureList;
        }

        // This method will be used to load the procedures from the database into the in memory repository
        public async Task LoadProceduresByDepartmentId(int departmentId)
        {
            try
            {
                s_procedureList.Clear();
                List<Procedure> procedures = await _medicalProcedureDBService.GetProceduresByDepartmentId(departmentId).ConfigureAwait(false);
                s_procedureList.Clear();
                foreach (Procedure procedure in procedures)
                {
                    s_procedureList.Add(procedure);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading procedures: {ex.Message}");
                return;
            }
        }



    }
}
