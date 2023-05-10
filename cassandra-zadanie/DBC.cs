using app3.Models;
using Cassandra;
using System.Xml.Linq;
using ISession = Cassandra.ISession;

namespace app3;

public class DBC
{
    private readonly ISession _connection;
    public DBC()
    {
        _connection = Cluster.Builder()
            .AddContactPoints("127.0.0.1")
            .WithCredentials("cassandra", "cassandra")
            .Build().Connect("myapp");
    }

    public RowSet Execute(string sql)
    {
        return _connection.Execute(sql);
    }

    public RowSet Execute(IStatement statement)
    {
        return _connection.Execute(statement);
    }

    public void AddUser(User u)
    {
        var stmt = _connection.Prepare("INSERT INTO Users (FirstName, LastName, Email, Password) values (?,?,?,?) ")
            .Bind(u.FirstName, u.LastName, u.Email, u.Password);
        Execute(stmt);
    }

    public bool Login(LoginDTO u)
    {
        var stmt = _connection.Prepare("SELECT * FROM Users where Email = ? AND Password = ?")
            .Bind(u.Email, u.Password);
        var res = Execute(stmt);
        int i = 0;
        foreach (var row in res.GetRows())
        {
            i++;
        }
        if (i == 0)
            return false;
        else
            return true;
    }

    public List<University> GetUniversities()
    {
        var l = new List<University>();
        var res = Execute("SELECT name FROM universities");
        foreach (var v in res.GetRows())
        {
            l.Add(new University() { Name = v[0].ToString() });
        }
        return l;
    }

    public void AddUniversity(University university)
    {
        var stmt = _connection.Prepare("INSERT INTO Universities (name) values (?)")
            .Bind(university.Name);
        Execute(stmt);
    }

    public List<Object> GetDepartments(string name)
    {
        var l = new List<Object>();
        var stm = _connection.Prepare("SELECT name FROM universitydepartments where universityname = ? ALLOW FILTERING").Bind(name);
        var res = Execute(stm);
        foreach (var v in res.GetRows())
        {
            l.Add(new { Name = v[0].ToString() });
        }
        return l;
    }
    public void AddDepartment(string uName, UniversityDepartment ud)
    {
        var stm = _connection.Prepare("INSERT INTO Universitydepartments (Universityname, name) values (?,?) ").Bind(uName, ud.Name);
        var res = Execute(stm);
    }

    public List<object> GetDepartmentFields(string name, string department)
    {
        var l = new List<Object>();
        var stm = _connection.Prepare("SELECT name, group FROM universitydepartmentfields where universityname = ? and universitydepartmentname = ? ALLOW FILTERING").Bind(name, department);
        var res = Execute(stm);
        foreach (var v in res.GetRows())
        {
            l.Add(new
            {
                Name = v[0].ToString(),
                Group = v[1].ToString()
            });
        }
        return l;
    }

    public void AddDepartmentField(string name, string department, UniversityDepartmentField udf)
    {
        var stm = _connection.Prepare("INSERT INTO UniversitydepartmentFields (Universityname, UniversityDepartmentName, name, group) values (?,?,?,?) ").Bind(name, department, udf.Name, udf.Group);
        var res = Execute(stm);
    }

    public List<object> GetUsers()
    {
        var l = new List<Object>();
        var stm = _connection.Prepare("SELECT firstname, lastname, email FROM users ").Bind();
        var res = Execute(stm);
        foreach (var v in res.GetRows())
        {
            l.Add(new
            {
                FirstName = v[0].ToString(),
                LastName = v[1].ToString(),
                Email = v[2].ToString()
            });
        }
        return l;
    }

    public UniversityResult SearchByName(string name)
    {
        var u = new UniversityResult();
        var prep = _connection.Prepare("SELECT name from universities where name = ?").Bind(name);
        var res = Execute(prep);
        foreach (var x in res.GetRows())
        {
            u.Name = x[0].ToString();
        };
        foreach (var x in res.GetRows())
        {
            
        };
        prep = _connection.Prepare("SELECT name from universitydepartments where universityname = ? ALLOW FILTERING").Bind(name);
        res = Execute(prep);
        u.Udr = new List<UniversityDepartmentResult>();
        foreach (var x in res.GetRows())
        {
            var q = new UniversityDepartmentResult();
            q.Name = x[0].ToString();
            q.Udfr = new List<UniversityDepartmentFieldResult>();
            prep = _connection.Prepare("SELECT name, group from universitydepartmentfields where universityname = ? and universitydepartmentname = ? ALLOW FILTERING").Bind(name, x[0].ToString());
            res = Execute(prep);
            foreach (var w in res)
            {
                q.Udfr.Add(new UniversityDepartmentFieldResult(){Name = w[0].ToString(), Group = w[1].ToString() });
            }
            u.Udr.Add(q);
        };
        return u;
    }
    public HashSet<string> SearchByGroup(string group)
    {
        var l = new HashSet<string>();
        var prep = _connection.Prepare("SELECT universityname from universitydepartmentfields where group = ? ALLOW FILTERING").Bind(group);
        var res = Execute(prep);
        foreach (var x in res.GetRows())
        {
            l.Add(x[0].ToString());
        };
        return l;
    }

    public List<string> GetFavourites(string email)
    {
        var l = new List<string>();
        var prep = _connection.Prepare("SELECT universityname from FavouriteUniversities where useremail = ? ALLOW FILTERING").Bind(email);
        var res = Execute(prep);
        foreach (var x in res.GetRows())
        {
            l.Add(x[0].ToString());
        };
        return l;
    }

    public void AddToFavourities(string email, string UniversityName)
    {
        var prep = _connection.Prepare("INSERT INTO FavouriteUniversities (universityname, UserEmail) values (?,?)")
            .Bind(UniversityName, email);
        _connection.Execute(prep);
    }
}