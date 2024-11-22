<!DOCTYPE html>
<html>
<head>
</head>
<body>
  <h1 align="center">🌟 WPF Application Setup Guide</h1>
  <p align="center">
    Step-by-step instructions to set up your WPF application with SQL Server database integration.
  </p>

  <hr>

  <h2>📋 Prerequisites</h2>
  <ul>
    <li>Install <b>.NET SDK</b> (<a href="https://dotnet.microsoft.com/download" target="_blank">Download here</a>).</li>
    <li>Ensure <b>SQL Server</b> is installed and running.</li>
    <li>Install an editor like <a href="https://visualstudio.microsoft.com/" target="_blank">Visual Studio</a> with WPF development tools enabled.</li>
    <li>Install <b>Entity Framework Core Tools If Needed</b>:
      <pre><code>dotnet tool install --global dotnet-ef</code></pre>
    </li>
  </ul>

  <h2>🚀 Steps to Set Up</h2>
  <h3>1️⃣ Connect Your Application to the Database</h3>
  <ol>
    <li>Go to the <code>appsettings.json</code> file located in your project directory.</li>
    <li>Update the connection string to match your SQL Server settings:
      <pre><code>
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server_name;Database=your_database_name;User Id=your_user;Password=your_password;"
  }
}
      </code></pre>
    </li>
    <li>Save the file.</li>
  </ol>

  <h3>2️⃣ Generate the Database</h3>
  <ol>
    <li>Open your terminal or Visual Studio Developer Command Prompt.</li>
    <li>Navigate to your project directory:
      <pre><code>cd your_project_directory</code></pre>
    </li>
    <li>Run the migration command to apply migrations and create the database:
      <pre><code>dotnet ef database update</code></pre>
    </li>
    <li>Confirm that the database schema has been created in your SQL Server.</li>
  </ol>

  <h3>3️⃣ Run Your WPF Application</h3>
  <ol>
    <li>Open the project in Visual Studio.</li>
    <li>Set the WPF project as the startup project.</li>
    <li>Run the application by pressing <code>F5</code> or using the "Start" button in Visual Studio.</li>
  </ol>

  <h3>4️⃣ Create an Admin Account</h3>
  <ol>
    <li>In the application, create a new user account through the registration form.</li>
    <li>Open your SQL Server database and locate the following tables:
      <ul>
        <li><b>ASPNETUSERROLE</b>: Assign the role <code>Admin</code> to your newly created user.</li>
        <li><b>ASPNETUSER</b>: Verify the user information is properly stored.</li>
      </ul>
    </li>
    <li>Save your changes and restart the application.</li>
  </ol>

  <h2>🎉 You're Ready to Go!</h2>
  <p>Congratulations! Your WPF application is now fully set up and ready for use. 🚀</p>

  <hr>

  <h2>🤝 Contributing</h2>
  <p>We welcome contributions! Fork the repository, create a new feature branch, and submit a pull request.</p>

  <h2>📧 Contact</h2>
  <p>If you have any questions or issues, feel free to reach out via <a href="mailto:phamlong070704@gmail.com">phamlong070704@gmail.com</a>.</p>
</body>
</html>
