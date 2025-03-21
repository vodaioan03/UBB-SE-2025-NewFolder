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
    public class DepartmentManagerModel
    {
        public static List<Department> s_departmentList { get; private set; }
        public readonly DepartmentsDatabaseService _departmentDBService;

        public DepartmentManagerModel(DepartmentsDatabaseService dbService)
        {
            _departmentDBService = dbService;
            s_departmentList = new List<Department>();
        }

        // This method will be used to get the departments from the in memory repository
        public List<Department> GetDepartments()
        {
            return s_departmentList;
        }


        // This method will be used to load the departments from the database into the in memory repository
        public async Task LoadDepartments()
        {

            s_departmentList.Clear();
            List<Department> departmentList = await _departmentDBService.GetDepartmentsFromDB().ConfigureAwait(false);
            foreach(Department dep in departmentList)
            {
                s_departmentList.Add(dep);
            }
        }

    }
}
