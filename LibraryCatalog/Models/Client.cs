using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace LibraryCatalog.Models
{
  public class Client
  {
    private int _id;
    private string _name;
    private string _dateofbirth;

    public Client(string name, string dateofbirth, int id = 0)
    {
      _id = id;
      _name = name;
      _dateofbirth = dateofbirth;
    }

    public override int GetHashCode()
    {
      return this.GetName().GetHashCode();
    }

    public string GetName()
    {
      return _name;
    }

    public string GetDateOfBirth()
    {
      return _dateofbirth;
    }

    public int GetId()
    {
      return _id;
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO client (name, date_of_birth) VALUES (@name, @dateofbirth);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter dateofbirth = new MySqlParameter();
      dateofbirth.ParameterName = "@dateofbirth";
      dateofbirth.Value = this._dateofbirth;
      cmd.Parameters.Add(dateofbirth);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
       conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Client> GetAll()
    {
      List<Client> allClients = new List<Client> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM client;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int clientId = rdr.GetInt32(0);
        string clientName = rdr.GetString(1);
        string clientDOB = rdr.GetString(2);
        Client newClient = new Client(clientName, clientDOB, clientId);
        allClients.Add(newClient);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allClients;
    }

    public static Client Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM client WHERE id = (@searchId);";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = id;
      cmd.Parameters.Add(searchId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int clientId = 0;
      string clientName = "";
      string clientDOB = "";

      while(rdr.Read())
      {
        clientId = rdr.GetInt32(0);
        clientName = rdr.GetString(1);
        clientDOB = rdr.GetString(2);
      }
      Client newClient = new Client(clientName, clientDOB, clientId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newClient;
    }

    public void CheckoutBook(Checkout newCheckout)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO checkout (book_id, client_id, date_borrowed, due_date, return_status) VALUES (@bookId, @clientId, @dateBorrowed, @dueDate, @returnStatus);";

      MySqlParameter bookId = new MySqlParameter();
      bookId.ParameterName = "@bookId";
      bookId.Value = newCheckout.GetBookId();
      cmd.Parameters.Add(bookId);

      MySqlParameter clientId = new MySqlParameter();
      clientId.ParameterName = "@clientId";
      clientId.Value = newCheckout.GetClientId();
      cmd.Parameters.Add(clientId);

      MySqlParameter dateBorrowed = new MySqlParameter();
      dateBorrowed.ParameterName = "@dateBorrowed";
      dateBorrowed.Value = newCheckout.GetDateBorrowed();
      cmd.Parameters.Add(dateBorrowed);

      MySqlParameter dueDate = new MySqlParameter();
      dueDate.ParameterName = "@dueDate";
      dueDate.Value = newCheckout.GetDueDate();
      cmd.Parameters.Add(dueDate);

      MySqlParameter returnStatus = new MySqlParameter();
      returnStatus.ParameterName = "@returnStatus";
      returnStatus.Value = newCheckout.GetReturnStatus();
      cmd.Parameters.Add(returnStatus);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
       conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void ReturnBook(Checkout returnBook)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE checkouts SET return_status =1 WHERE id = @checkoutId;";

      MySqlParameter checkoutId = new MySqlParameter();
      checkoutId.ParameterName = "@checkoutId";
      checkoutId.Value = returnBook.GetId();
      cmd.Parameters.Add(checkoutId);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Book> GetBorrowHistory()
    {
      List<Book> allBook = new List<Book> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT books.* FROM checkouts JOIN books ON (checkouts.book_id = books.id) WHERE checkouts.client_id = @clientId;";

      MySqlParameter clientId = new MySqlParameter();
      clientId.ParameterName = "@clientId";
      clientId.Value = this._id;
      cmd.Parameters.Add(clientId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int BookId = rdr.GetInt32(0);
        string BookName = rdr.GetString(1);
        int BookCopies = rdr.GetInt32(2);
        Book newBook = new Book(BookName, BookCopies, BookId);
        allBook.Add(newBook);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allBook;
    }

    public void DeleteClient()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand("DELETE FROM client WHERE id = @clientId; DELETE FROM checkouts WHERE client_id = @ClientId;", conn);
      MySqlParameter ClientIdParameter = new MySqlParameter();
      ClientIdParameter.ParameterName = "@ClientId";
      ClientIdParameter.Value = this.GetId();

      cmd.Parameters.Add(ClientIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
