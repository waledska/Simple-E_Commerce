using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace SimpleECommerce.EmailForms
{
    public class GenerateMail
    {
        // Correctly defined constructor
        public string ResetPassOTP(string otp)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; color: #444; }}
        .container {{ max-width: 600px; margin: 20px auto; padding: 20px; background-color: #ffffff; border: 1px solid #ddd; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .otp {{ font-weight: bold; margin: 20px 0; color: #007bff; font-size: 20px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #888; }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>OTP Request</h1>
        <p>Hi,</p>
        <p>You requested to reset your password or Confirm Email. Here's your One-Time Password (OTP) to complete the process:</p>
        <p class=""otp"">OTP: {otp}</p>
        <p>This code is valid for 5 minutes. If you didn't request this, please ignore this email or contact support if you feel something is wrong.</p>
        <div class=""footer"">
            Thanks,<br>
            [SAHM]
        </div>
    </div>
</body>
</html>";
        }

    }
}
