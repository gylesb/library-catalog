using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace Library.Models
{
  public class Book
  {
    private int _id;
    private string _title;
    private int _copies;

    public Book(string title, int copies, int id = 0)
    {
      _id = id;
      _title = title;
      _copies = copies;
    }

    public override bool Equals(System.Object otherBook)
    {
      if(!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool idEquality = this.GetId() == newBook.GetId();
        bool titleEquality = this.GetTitle() == newBook.GetTitle();
        bool copiesEquality = this.GetCopies() == newBook.GetCopies();
        return(idEquality && titleEquality && copiesEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.GetTitle().GetHashCode();
    }

    public string GetTitle()
    {
      return _title;
    }

    public int GetId()
    {
      return _id;
    }

    public int GetCopies()
    {
      return _copies;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books (title, number_of_copies) VALUES (@title, @copies); INSERT INTO available_copies (book_id, number_of_copies) VALUES (@bookId, @copies);";

      MySqlParameter title = new MySqlParameter();
      title.ParameterName = "@title";
      title.Value = this._title;
      cmd.Parameters.Add(title);

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = this._id;
      cmd.Parameters.Add(bookId);

      MySqlParameter copies = new MySqlParameter();
      copies.ParameterName = "@copies";
      copies.Value = this._copies;
      cmd.Parameters.Add(copies);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void AddAuthor(Author newAuthor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO books_authors (book_id, author_id) VALUES (@bookId, @authorId);";

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = this._id;
      cmd.Parameters.Add(bookId);

      MySqlParameter authorId = new MySqlParameter();
      authorId.ParameterName = "@authorId";
      authorId.Value = newAuthor.GetId();
      cmd.Parameters.Add(authorId);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Book> GetAll()
    {
      List<Book> allBook = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        int bookCopies = rdr.GetInt32(2);
        Book newBook = new Book(bookTitle, bookCopies, bookId);
        allBook.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allBook;
    }

    public static List<Author> GetAuthors()
    {
      List<Author> bookAuthors = new List<Author> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT authors.* FROM books JOIN books_authors ON (books.id = books_authors.book_id) JOIN authors (books_authors.author_id = authors.id) WHERE book.id = (@searchId);";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorTitle = rdr.GetString(1);
        int authorCopies = rdr.GetInt32(2);
        Author newAuthor = new Author(authorTitle, authorCopies, authorId);
        bookAuthors.Add(newAuthor);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return bookAuthors;
    }

    public static Book Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int bookId = 0;
      string bookTitle = "";
      int bookCopies = 0;

      while(rdr.Read())
      {
        bookId = rdr.GetInt32(0);
        bookTitle = rdr.GetString(1);
        bookCopies = rdr.GetInt32(2);
      }
      Book newBook = new Book(bookTitle, bookCopies, bookId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newBook;
    }

    public static List<Book> SearchTitle(string title)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM books WHERE title = (@searchTitle);";

      MySqlParameter searchTitle = new MySqlParameter();
      searchTitle.ParameterName = "@searchTitle";
      searchTitle.Value = title;
      cmd.Parameters.Add(searchTitle);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Book> books = new List<Book>{};

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        int bookCopies = rdr.GetInt32(2);
        Book newBook = new Book(bookTitle, bookCopies, bookId);
        books.Add(newBook);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }

    public static List<Book> SearchTitle(Author newAuthor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT books.* FROM authors
      JOIN books_authors ON (authors.id = books_authors.author_id) JOIN books ON (books_authors.book_id = books.id) WHERE authors.name = (@searchAuthor);";

      MySqlParameter searchAuthor = new MySqlParameter();
      searchAuthor.ParameterName = "@searchAuthor";
      searchAuthor.Value = newAuthor.GetName();
      cmd.Parameters.Add(searchAuthor);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<Book> books = new List<Book>{};

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        int bookCopies = rdr.GetInt32(2);
        Book newBook = new Book(bookTitle, bookCopies, bookId);
        books.Add(newBook);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return books;
    }

    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM books;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void DeleteBook()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM books WHERE id = @bookId; DELETE FROM books_authors WHERE book_id = @BookId;", conn);
      MySqlParameter BookIdParameter = new MySqlParameter();
      BookIdParameter.ParameterName = "@BookId";
      BookIdParameter.Value = this.GetId();

      cmd.Parameters.Add(BookIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
