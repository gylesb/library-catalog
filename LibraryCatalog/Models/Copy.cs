using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace LibraryCatalog.Models
{
  public class Copy
  {
    private int _id;
    private string _bookTitle;
    private int _copies;

    public Copy(string bookTitle, int copies, int Id = 0)
    {
      _id = Id;
      _bookTitle = bookTitle;
      _copies = copies;
    }

    public override bool Equals(System.Object otherCopy)
    {
      if (!(otherCopy is Copy))
      {
        return false;
      }
      else
      {
        Copy newCopy = (Copy) otherCopy;
        bool idEquality = (this.GetId() == newCopy.GetId());
        bool bookTitleEquality = (this.GetId() == newCopy.GetBookTitle());
        bool copiesEquality = (this.GetId == newCopy.GetCopies());

        return (idEquality && bookTitleEquality && copiesEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetBookTitle().GetHashCode();
    }

    public int GetId()
    {
      return _id;
    }

    public string GetBookTitle()
    {
      return _bookTitle;
    }

    public string GetCopies()
    {
      return _copies;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO copies (book, copies) VALUES (@book, @copies);";

      MySqlParameter bookTitle = new MySqlParameter();
      bookTitle.ParameterName = "@bookTitle";
      bookTitle.Value = this._bookTitle;
      cmd.Parameters.Add(bookTitle);

      MySqlParameter copiesAdd = new MySqlParameter();
      copiesAdd.ParameterName = "@copiesAdd";
      copiesAdd.Value = this._copies;
      cmd.Parameters.Add(copiesAdd);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Copy Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM 'copies' where id = @thisId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@thisId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int copiesId = 0;
      string copiesTitle = "";
      int copiesCopy = 0;

      while (rdr.Read())
      {
        copiesId = rdr.GetInt32(0);
        copiesTitle = rdr.GetString(1);
        copiesCopy = rdr.GetInt(2);
      }

      Copy newCopy = new Copy(copiesId, copiesTitle, copiesCopy);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newCopy;
    }

    public List<Book> GetBooks()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT title_id FROM library_copies WHERE copies_id = @copiesId;";

      MySqlParameter copiesIdParameter = new MySqlParameter();
      copiesIdParameter.ParameterName = "@copiesId";
      copiesIdParameter.Value = _id;
      cmd.Parameters.Add(copiesIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> bookIds = new List<int> {};
      while (rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        BookIds.Add(bookId);
      }
      rdr.Dispose();

      List<Book> books = new List<Book> {};
      foreach (int bookId in bookIds)
      {
        var bookQuery = conn.CreateCommand() as MySqlCommand;
        bookQuery.CommandText = @"SELECT * FROM library WHERE id = @BookId;";

        MySqlParameter bookIdParameter = new MySqlParameter();
        bookIdParameter.ParameterName = "@BookId";
        bookIdParameter.Value = bookId;
        bookQuery.Parameters.Add(bookIdParameter);

        var bookQueryRdr = bookQuery.ExecuteReader() as MySqlDataReader;

        while(bookQueryRdr.Read())
        {
          int thisBookId = bookQueryRdr.GetInt32(0);
          string bookTitle = bookQueryRdr.GetString(1);
          string bookAuthor = bookQueryRdr.GetString(2);
          Book foundBook = new Book(bookTitle, bookAuthor, thisBookId);
          books.Add(foundBook);
        }
        bookQueryRdr.Dispose();
      }
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }

    public List<Book> GetAvailableBooks()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT course_id FROM courses WHERE student_id = @studentId;";

      MySqlParameter studentIdParameter = new MySqlParameter();
      studentIdParameter.ParameterName = "@studentId";
      studentIdParameter.Value = _id;
      cmd.Parameters.Add(studentIdParameter);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<int> courseIds = new List<int> {};
      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        courseIds.Add(courseId);
      }
      rdr.Dispose();

      List<Book> books = new List<Book> {};
      foreach (int bookId in bookIds)
      {
        var bookQuery = conn.CreateCommand() as MySqlCommand;
        bookQuery.CommandText = @"SELECT * FROM books WHERE id = @BookId;";

        MySqlParameter bookIdParameter = new MySqlParameter();
        bookIdParameter.ParameterName = "@BookId";
        bookIdParameter.Value = bookId;
        bookQuery.Parameters.Add(bookIdParameter);

        var bookQueryRdr = bookQuery.ExecuteReader() as MySqlDataReader;
        while(bookQueryRdr.Read())
        {
          int thisBookId = bookQueryRdr.GetInt32(0);
          string bookName = bookQueryRdr.GetString(1);
          string bookNumber = bookQueryRdr.GetString(2);
          Book foundBook = new Book(bookName, bookNumber, thisBookId);
          books.Add(foundBook);
        }
        bookQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }
    // public static List<Student> GetAll()
    // {
    //   List<Student> allStudents = new List<Student> {};
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //   var cmd = conn.CreateCommand() as MySqlCommand;
    //   cmd.CommandText = @"SELECT * FROM students ORDER BY name ASC;";
    //   var rdr = cmd.ExecuteReader() as MySqlDataReader;
    //   while(rdr.Read())
    //   {
    //     int studentId = rdr.GetInt32(0);
    //     string studentName = rdr.GetString(1);
    //     string studentEnrolmentDate = rdr.GetString(2);
    //     Student newStudent = new Student(studentName, studentEnrolmentDate, studentId);
    //     allStudents.Add(newStudent);
    //   }
    //   conn.Close();
    //   if (conn != null)
    //   {
    //     conn.Dispose();
    //   }
    //   return allStudents;
    // }
    //
    // public void AddCourse(Course newCourse)
    // {
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //   var cmd = conn.CreateCommand() as MySqlCommand;
    //   cmd.CommandText = @"INSERT INTO courses_students (course_id, student_id) VALUES (@CourseId, @StudentId);";
    //
    //   MySqlParameter course_id = new MySqlParameter();
    //   course_id.ParameterName = "@CourseId";
    //   course_id.Value = newCourse.GetId();
    //   cmd.Parameters.Add(course_id);
    //
    //   MySqlParameter student_id = new MySqlParameter();
    //   student_id.ParameterName = "@StudentId";
    //   student_id.Value = _id;
    //   cmd.Parameters.Add(student_id);
    //
    //   cmd.ExecuteNonQuery();
    //   conn.Close();
    //   if (conn != null)
    //   {
    //     conn.Dispose();
    //   }
    // }
    //
    // public void Delete()
    // {
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //   var cmd = conn.CreateCommand() as MySqlCommand;
    //   cmd.CommandText = @"DELETE FROM students WHERE id = @StudentId; DELETE FROM Courses_students WHERE student_id = @StudentId;";
    //
    //   MySqlParameter studentIdParameter = new MySqlParameter();
    //   studentIdParameter.ParameterName = "@StudentId";
    //   studentIdParameter.Value = this.GetId();
    //   cmd.Parameters.Add(studentIdParameter);
    //
    //   cmd.ExecuteNonQuery();
    //   if (conn != null)
    //   {
    //     conn.Close();
    //   }
    // }
    //
    // public static void DeleteAll()
    // {
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //   var cmd = conn.CreateCommand() as MySqlCommand;
    //   cmd.CommandText = @"DELETE FROM students;";
    //   cmd.ExecuteNonQuery();
    //   conn.Close();
    //   if (conn != null)
    //   {
    //     conn.Dispose();
    //   }
    // }
  }
}
