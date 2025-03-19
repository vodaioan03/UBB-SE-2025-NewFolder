using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
  public class DoctorManagerModel
  {
    public ObservableCollection<DoctorJointModel> s_doctorList { get; private set; }
    private DoctorsDatabaseService _doctorDBService;

    public DoctorManagerModel(DoctorsDatabaseService dbService)
    {
      _doctorDBService = dbService;
    }

    public async Task LoadDoctors(int departmentId)
    {
      try
      {
        s_doctorList.Clear();
        List<DoctorJointModel> doctorsList = await _doctorDBService.GetDoctorsByDepartment(departmentId).ConfigureAwait(false);
        foreach (DoctorJointModel doc in doctorsList)
        {
          s_doctorList.Add(doc);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading departments: {ex.Message}");
      }
    }

    ObservableCollection<DoctorJointModel> GetDoctorsWithRatings()
    {
      return s_doctorList;
    }
  }
}
