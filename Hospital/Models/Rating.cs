using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int MedicalRecordId { get; set; }
        public float NrStars { get; set; }
        public string Motivation { get; set; }

        public Rating(int ratingId, int medicalRecordId, float nrStars, string motivation)
        {
            RatingId = ratingId;
            MedicalRecordId = medicalRecordId;
            NrStars = nrStars;
            Motivation = motivation;
        }
    }
}
