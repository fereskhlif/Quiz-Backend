namespace Quizzz.Helper
{
    public static class EmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"">
  <title>Reset Your Password</title>
  <style>
    body {{
      margin: 0;
      padding: 0;
      font-family: Arial, Helvetica, sans-serif;
      background: linear-gradient(to top, #c9c9ff 50%, #6e6ef6 90%) no-repeat;
      height: auto;
    }}
    .container {{
      padding: 40px;
      background-color: white;
      max-width: 600px;
      margin: 50px auto;
      border-radius: 8px;
      box-shadow: 0 0 10px rgba(0,0,0,0.1);
    }}
    h1 {{
      color: #333;
    }}
    p {{
      font-size: 16px;
      color: #555;
    }}
    .btn {{
      display: inline-block;
      padding: 10px 20px;
      background-color: #6e6ef6;
      color: white;
      text-decoration: none;
      border-radius: 5px;
      margin-top: 20px;
    }}
  </style>
</head>
<body>
  <div class=""container"">
    <h1>Reset your Password</h1>
    <p>You’re receiving this e-mail because you requested a password reset for your Let's Program account.</p>
    <p>Please tap the button below to choose a new password.</p>
    <a href=""https://localhost:4200/reset-password?email={email}&token={emailToken}"" class=""btn"">Reset Password</a>
    <p>Kind Regards,<br></br>
</p>  
</div>
</body>
</html>";
        }
    }
}
