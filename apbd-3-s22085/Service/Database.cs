namespace apbd_3_s22085.Service;

public interface IDatabase<T>
{
    IEnumerable<T> GetStudents();
    T? GetStudent(string id);
    Task<T> CreateStudent(Student student);
    Task<T> UpdateStudent(string id, Student student);
    Task<T?> DeleteStudent(string id);
}

public class Database : IDatabase<Student>
{
    private readonly string dbPath = $"{Environment.CurrentDirectory}\\db.csv";
    
    public IEnumerable<Student> GetStudents()
    {
        return getStudents();
    }

    public Student? GetStudent(string id)
    {
        return getStudents().FirstOrDefault(s => s.IndexNumber == id);
    }

    public async Task<Student> CreateStudent(Student student)
    {
        await File.AppendAllTextAsync(
            dbPath,
            Environment.NewLine + string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", 
                student.FirstName, 
                student.Surname,
                student.IndexNumber, 
                student.BirthDate, 
                student.StudiesName, 
                student.StudiesMode, 
                student.Email,
                student.FathersName, 
                student.MothersName)
        );
        return student;
    }

    public async Task<Student?> UpdateStudent(string id, Student student)
    {
        var reader = new StreamReader(dbPath);
        var lines = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) // skip empty lines
            {
                continue;
            }
            var columns = line.Split(',');
            if (columns[2] == id)
            {
                line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", 
                    student.FirstName, 
                    student.Surname,
                    columns[2], // do not update the index number 
                    student.BirthDate, 
                    student.StudiesName, 
                    student.StudiesMode, 
                    student.Email,
                    student.FathersName, 
                    student.MothersName);
                student.IndexNumber = columns[2]; // return the original index number
            }
            lines.Add(line);
        }
        reader.Close(); // implicit close so that we can write to the file
        await WriteAllLinesWithoutEmptyLine(dbPath, lines);
        
        return student;
    }

    private static async Task WriteAllLinesWithoutEmptyLine(string path, List<string> lines)
    {
        if (path == null)
            throw new ArgumentNullException("path");
        if (lines == null)
            throw new ArgumentNullException("lines");

        using var stream = File.OpenWrite(path);
        using var writer = new StreamWriter(stream);
        
        if (lines.Count > 0)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                await writer.WriteLineAsync(lines[i]);
            }
            await writer.WriteAsync(lines[lines.Count - 1]);
        }
    }

    public async Task<Student?> DeleteStudent(string id)
    {
        var reader = new StreamReader(dbPath);
        var lines = new List<string>();
        Student? student = null;
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) // skip empty lines
            {
                continue;
            }
            var columns = line.Split(',');
            if (columns[2] == id)
            {
                student = new Student
                {
                    FirstName = columns[0],
                    Surname = columns[1],
                    IndexNumber = columns[2],
                    BirthDate = columns[3],
                    StudiesName = columns[4],
                    StudiesMode = columns[5],
                    Email = columns[6],
                    FathersName = columns[7],
                    MothersName = columns[8]
                };
                continue;
            }
            lines.Add(line);
        }
        reader.Close(); // implicit close so that we can write to the file
        await File.WriteAllLinesAsync(dbPath, lines);
        
        return student;
        
    }

    private List<Student> getStudents()
    {
        using var reader = new StreamReader(dbPath);
        var students = new List<Student>();
        
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            var columns = line.Split(',');
            students.Add(new Student
            {
                FirstName = columns[0],
                Surname = columns[1],
                IndexNumber = columns[2],
                BirthDate = columns[3],
                StudiesName = columns[4],
                StudiesMode = columns[5],
                Email = columns[6],
                FathersName = columns[7],
                MothersName = columns[8]
            });
        }

        return students;
    }
    
    
}