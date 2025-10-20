using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tas.Domain.Common;

namespace tas.Application.Utils
{
    public class EmailTemplateService
    {


        public EmailTemplateService()
        {
        }

        public string LoadTemplate(string templateFileName)
        {
            try
            {



               return MailTemplate.REQUEST_DOCUMENT_MAIL_TEMPLATE;
                //string filePath = Path.Combine("Resources", $"mailtemplates/{templateFileName}");

                //if (File.Exists(filePath))
                //{
                //    return File.ReadAllText(filePath);
                //}
                //else
                //{
                //    //throw new FileNotFoundException($"Template file not found: {templateFileName}");
                //    return string.Empty;
                //}
            }
            catch (Exception ex)
            {
                //// Handle file loading errors
                //throw new Exception($"Failed to load template: {ex.Message}", ex);

                return string.Empty;
            }
        }
    }
}
