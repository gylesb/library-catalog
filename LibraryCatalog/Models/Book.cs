using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace LibraryCatalog.Models
{
  public class Book
  {
    private string _title;
    private string _author;
    private int _id;

    public Book(string title, string author, int id = 0)
    {
      _title = title;
      _author = author;
      _id = id;
    }

    public override bool Equals(System.Object otherBook)
    {
      if (!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        return this.GetId().Equals(newBook.GetId());
      }
    }

    public override int GetHashCode()
    {
      return this.GetId().GetHashCode();
    }

    public string GetBookTitle()
    {
      return _title;
    }

    public string GetBookAuthor()
    {
      return _author;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM library;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int BookId = rdr.GetInt32(0);
        string BookTitle = rdr.GetString(1);
        string BookAuthor = rdr.GetString(2);

        Book newBook = new Book(BookTitle, BookAuthor, BookId);
        allBooks.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allBooks;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO library (title, author) VALUES (@title, @author);";

      MySqlParameter bookTitle = new MySqlParameter();
      bookTitle.ParameterName = "@bookTitle";
      bookTitle.Value = this._title;
      cmd.Parameters.Add(bookTitle);

      MySqlParameter bookAuthor = new MySqlParameter();
      bookAuthor.ParameterName = "@bookAuthor";
      bookAuthor.Value = this._author;
      cmd.Parameters.Add(bookAuthor);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Book Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM library WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int BookId = 0;
      string BookTitle = "";
      string BookAuthor = "";

      while (rdr.Read())
      {
        BookId = rdr.GetInt32(0);
        BookTitle = rdr.GetString(1);
        BookAuthor = rdr.GetString(2);
      }

      Book newBook = new Book(BookTitle, BookAuthor);
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
      return newBook;
    }

    public void AddCopy(Copy newCopy)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO library_copies (title_id, author_id, copies_id) VALUES (@TitleId, @AuthorId, @CopiesId);";

      MySqlParameter title_id = new MySqlParameter();
      title_id.ParameterName = "@TitleId";
      title_id.Value = _id;
      cmd.Parameters.Add(title_id);

      MySqlParameter author_id = new MySqlParameter();
      author_id.ParameterName = "@AuthorId";
      author_id.Value = _id;
      cmd.Parameters.Add(author_id);

      MySqlParameter copies_id = new MySqlParameter();
      copies_id.ParameterName = "@CopiesId";
      copies_id.Value = _id;
      cmd.Parameters.Add(copies_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM library WHERE id = @CopiesId; DELETE FROM copies WHERE copies_id = @CopiesId;", conn);
      MySqlParameter copiesIdParameter = new MySqlParameter();
      copiesIdParameter.ParameterName = "@CopiesId";
      copiesIdParameter.Value = this.GetId();

      cmd.Parameters.Add(copiesIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM library;";
      cmd.ExecuteNonQuery();
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
    }


  }
}
