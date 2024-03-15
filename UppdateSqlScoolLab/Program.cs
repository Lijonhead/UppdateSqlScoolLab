using System.Data.SqlClient;
using System.Globalization;
using System.Xml.Linq;

namespace UppdateSqlScoolLab
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectiongString = @"Data Source=(localdb)\.;Initial Catalog=UppdatedSqlSchool;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectiongString))
            {
                connection.Open();
                bool exit = false;
                do
                {
                    Console.Clear();
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Get all students");
                Console.WriteLine("2. Get all students in a certain class");
                Console.WriteLine("3. Add new staff");
                Console.WriteLine("4. Get staff");
                Console.WriteLine("5. Get all grades set in the last month");
                Console.WriteLine("6. Average grade per course");
                Console.WriteLine("7. Add new students");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                

                
                   

                    string userInput = Console.ReadLine();

                    switch (userInput)
                    {
                        case "1":
                            Console.Clear();
                            //Get All students and chose sorting order
                            Console.WriteLine("Enter sorting order 1 or 2 (ASC or DESC):");
                            string sortOrder = Console.ReadLine();
                            if (sortOrder != "1" && sortOrder != "2")
                            {
                                Console.WriteLine("Invalid sorting order. Defaulting to ASC.");
                                sortOrder = "ASC"; 
                            }
                            if (sortOrder == "1")
                            {
                                sortOrder = "ASC";
                            }
                            if (sortOrder == "2")
                            {
                                sortOrder = "DESC";
                            }

                            Console.WriteLine("Enter column to sort by 1 or 2 (FirstName or LastName):");
                            string sortBy = Console.ReadLine();                        

                            if (sortBy != "1" && sortBy != "2")
                            {
                                Console.WriteLine("Invalid column to sort by. Defaulting to FirstName.");
                                sortBy = "FirstName"; 
                            }
                            if (sortBy == "1")
                            {
                                sortBy = "FirstName";
                            }
                            if (sortBy == "2")
                            {
                                sortBy = "LastName";
                            }
                            using (SqlCommand command = new SqlCommand($"SELECT * FROM Students ORDER BY {sortBy} {sortOrder}", connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName")).TrimEnd();
                                        string LastName = reader.GetString(reader.GetOrdinal("LastName")).TrimEnd();
                                        Console.WriteLine($"{FirstName} {LastName}");

                                    }
                                    
                                }

                            }
                            break;
                        case "2":
                            Console.Clear();
                            // Gets all students in a certain class

                            Console.WriteLine("Enter class number: ");
                            using (SqlCommand command = new SqlCommand("SELECT * FROM Classes ", connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string ClassName = reader.GetString(reader.GetOrdinal("ClassName")).TrimEnd();
                                        
                                       
                                        Console.WriteLine($"{ClassName}");

                                    }

                                }

                            }
                             
                           string ClassId =  Console.ReadLine();
                            using (SqlCommand command = new SqlCommand($"SELECT * FROM Students WHERE Class_Id={ClassId} ", connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName")).TrimEnd();
                                        string LastName = reader.GetString(reader.GetOrdinal("LastName")).TrimEnd();
                                        Console.WriteLine($"{FirstName} {LastName}");

                                    }

                                }

                            }
                            break;
                        case "3":
                            Console.Clear();
                            Console.WriteLine("Enter first name ");
                           string PersonnelFirstName = Console.ReadLine();
                            Console.WriteLine("Enter last name ");
                           string PersonnelLastName = Console.ReadLine();
                            Console.WriteLine("Enter job title");
                            string PersonnelJob = Console.ReadLine();

                            using (SqlCommand command = new SqlCommand("INSERT INTO Personnel (FirstName, LastName, Job) VALUES (@FirstName, @LastName, @Job); ", connection))
                            {
                                command.Parameters.AddWithValue("@FirstName", PersonnelFirstName);
                                command.Parameters.AddWithValue("@LastName", PersonnelLastName);
                                command.Parameters.AddWithValue("@Job", PersonnelJob);

                                try
                                {
                                    
                                    int rowsAffected = command.ExecuteNonQuery();

                                    
                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("New staff member added successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to add new staff member.");
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    
                                    Console.WriteLine("An error occurred while adding the new staff member: " + ex.Message);
                                }

                            }

                            break;
                        case "4":
                            Console.Clear();
                            Console.WriteLine("Do you want to see all Personnel or only within a certain proffesion? 1 or 2 (All/Some)");
                            string choice = Console.ReadLine();
                            if(choice == "1")
                            {
                                using (SqlCommand command = new SqlCommand("SELECT * FROM Personnel", connection))
                                {
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string FirstName = reader.GetString(reader.GetOrdinal("FirstName")).TrimEnd();
                                            string LastName = reader.GetString(reader.GetOrdinal("LastName")).TrimEnd();
                                            string Job = reader.GetString(reader.GetOrdinal("Job")).TrimEnd();
                                            Console.WriteLine($"{FirstName} {LastName} {Job}");

                                        }

                                    }

                                }
                            }
                            if(choice == "2")
                            {
                                Console.WriteLine("What proffesion do you want to see from? input Teacher, Principle, Janitor");
                                string secondChoice = Console.ReadLine();
                                using (SqlCommand command = new SqlCommand($"SELECT * FROM Personnel WHERE Job='{secondChoice}'", connection))
                                {
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string FirstName = reader.GetString(reader.GetOrdinal("FirstName")).TrimEnd();
                                            string LastName = reader.GetString(reader.GetOrdinal("LastName")).TrimEnd();
                                            string Job = reader.GetString(reader.GetOrdinal("Job")).TrimEnd();
                                            Console.WriteLine($"{FirstName} {LastName} {Job}");

                                        }

                                    }

                                }
                            }

                            break;
                        case "5":
                            Console.Clear();
                            DateTime today = DateTime.Now;
                            DateTime month = DateTime.Now.AddMonths(-1);
                            using (SqlCommand command = new SqlCommand($"SELECT Students.FirstName, Courses.CourseName, Grades.Grade FROM Grades INNER JOIN Students ON Grades.Student_Id=Students.Id INNER JOIN Courses ON Grades.Course_Id=Courses.Id WHERE GradeDate BETWEEN '2024-02-15 22:12:08' AND '2024-03-15 22:12:08'", connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName")).TrimEnd();
                                        string CourseName = reader.GetString(reader.GetOrdinal("CourseName")).TrimEnd();
                                        int Grade = reader.GetInt32(reader.GetOrdinal("Grade"));
                                        Console.WriteLine($"Name:{FirstName} Course:{CourseName} Grade:{Grade}");

                                    }

                                }

                            }


                            break;
                        case "6":
                            Console.Clear();
                            using (SqlCommand command = new SqlCommand($"SELECT Courses.CourseName, AVG(Grades.Grade) AS AverageGrade, MAX(Grades.Grade) AS HighestGrade, MIN(Grades.Grade) AS LowestGrade  FROM Courses INNER JOIN Grades ON Courses.Id=Grades.Course_Id GROUP BY Courses.CourseName", connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    Console.WriteLine("Course\t\tAverage Grade\tHighest Grade\tLowest Grade");
                                    Console.WriteLine("------------------------------------------------------------");
                                    while (reader.Read())
                                    {
                                        string CourseName = reader.GetString(reader.GetOrdinal("CourseName")).TrimEnd();
                                        int averageGrade = reader.GetInt32(reader.GetOrdinal("AverageGrade"));
                                        int highestGrade = reader.GetInt32(reader.GetOrdinal("HighestGrade"));
                                        int lowestGrade = reader.GetInt32(reader.GetOrdinal("LowestGrade"));
                                        Console.WriteLine($"{CourseName}\t\t{averageGrade}\t\t{highestGrade}\t\t{lowestGrade}");

                                    }

                                }

                            }

                            break;
                        case "7":
                            Console.Clear();
                            Console.WriteLine("Enter details of the new student:");
                            Console.Write("First Name: ");
                            string firstName = Console.ReadLine();
                            Console.Write("Last Name: ");
                            string lastName = Console.ReadLine();
                            Console.Write("Class Id: ");
                            int studentClass = Convert.ToInt32(Console.ReadLine());

                            // SQL query to insert the new student into the database
                            

                            // Create and execute the SQL command with parameters
                            using (SqlCommand command = new SqlCommand("INSERT INTO Students (FirstName, LastName, Class_Id) VALUES (@FirstName, @LastName, @Class_Id)", connection))
                            {
                                // Add parameters to the command
                                command.Parameters.AddWithValue("@FirstName", firstName);
                                command.Parameters.AddWithValue("@LastName", lastName);
                                command.Parameters.AddWithValue("@Class_Id", studentClass);

                                try
                                {
                                    // Execute the SQL command
                                    int rowsAffected = command.ExecuteNonQuery();

                                    // Check if the insertion was successful
                                    if (rowsAffected > 0)
                                    {
                                        Console.WriteLine("New student added successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to add new student.");
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    // Handle any SQL exceptions
                                    Console.WriteLine("An error occurred while adding the new student: " + ex.Message);
                                }
                            }

                            break;
                        case "0":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }

                    Console.WriteLine("Press Enter to return to the main menu.");
                    Console.ReadLine();
                } while (!exit);


            }
        }
    }
}
