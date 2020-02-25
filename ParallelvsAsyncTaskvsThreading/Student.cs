using System;
using System.Collections.Generic;
using System.Text;

namespace ParallelvsAsyncTaskvsThreading
{
   public class Student
    {
        public int Id { set; get; }
        public string Name { set; get; }

        public int Age { set; get; }
        public override string ToString()
        {
            return "Student: " + this.Id + " Name " + this.Name + " Age " + this.Age;
        }
    }
}
