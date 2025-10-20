using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using tas.Application.Extensions;
using tas.Application.Features.CampFeature.CreateCamp;
using tas.Application.Features.RoomFeature.CreateRoom;
using tas.Application.Repositories;
using tas.Application.Service;
using tas.Domain.CustomModel;
using tas.Domain.Entities;
using tas.WebAPI.Extensions;
using static ClosedXML.Excel.XLPredefinedFormat;

namespace tas.WebAPI.Controllers.Auth
{
    [Route("api/tas/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly HTTPUserRepository _hTTPUserRepository;
        private readonly IMediator _mediator;
        public TestController(IMediator mediator, IConfiguration configuration, ILogger<AuthController> logger, HTTPUserRepository hTTPUserRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _hTTPUserRepository = hTTPUserRepository;
            _mediator = mediator;

        }



        //[HttpGet]
        //[Route("/")]
        //public IActionResult GetData()
        //{
        //    try
        //    {

        //        return Ok("test");
               

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}




        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload([FromForm] MyModel model)
        {
            try
            {
                
                var fmodel = model.File.SaveImageFile();
                if (fmodel.Status)
                {
                    return Ok(fmodel.FileName);
                }
                else {
                    return StatusCode(400, fmodel.Error);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            var testdata = new List<Employee>()
        {
            new Employee(){ Id=101, Lastname="Johnny", RosterId = 1},
            new Employee(){ Id=102, Lastname="Tom", RosterId = 1},
            new Employee(){ Id=103, Lastname="Jack", RosterId = 1},
            new Employee(){ Id=104, Lastname="Vivian", RosterId = 2},
            new Employee(){ Id=105, Lastname="Edward", RosterId = 3},
        };

            var RosterData = new List<Roster>() {
                new Roster() { Id = 1, Name = "Test 1" },
                new Roster() { Id = 2, Name = "Test 2" },
                new Roster() { Id = 3, Name = "Test 3" }
                };


            


            //using System.Data;  
            DataTable dt = new DataTable("EmployeeInfo");
            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("Id"),
                                    new DataColumn("Lastname"),
                                    new DataColumn("RosterName")
            
            });

            foreach (var emp in testdata)
            {
                dt.Rows.Add(emp.Id, emp.Lastname, emp.RosterId);
            }

            DataTable dtRoster = new DataTable("RosterInfo");
            dtRoster.Columns.AddRange(new DataColumn[2] { new DataColumn("Id"),
                                    new DataColumn("RosterName")

            });


            foreach (var roster in RosterData)
            {
                dtRoster.Rows.Add(roster.Id, roster.Name);
            }


            foreach (var emp in testdata)
            {
                dt.Rows.Add(emp.Id, emp.Lastname, emp.RosterId);
            }

            //using ClosedXML.Excel;  
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Worksheets.Add(dtRoster);
 


                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        [HttpGet("ExportToExcel2")]
        public IActionResult ExportToExcel2()
        {
            var wb = new XLWorkbook();
            IXLWorksheet ws;

          
            String multiColumn = "Multi Column";
            ws = wb.Worksheets.Add(multiColumn);

            ws.Cell("A1").SetValue("First")
             .CellBelow().SetValue("B")
             .CellBelow().SetValue("C")
             .CellBelow().SetValue("C")
             .CellBelow().SetValue("E")
             .CellBelow().SetValue("A")
             .CellBelow().SetValue("D");

            ws.Cell("B1").SetValue("Numbers")
                         .CellBelow().SetValue(2)
                         .CellBelow().SetValue(3)
                         .CellBelow().SetValue(3)
                         .CellBelow().SetValue(5)
                         .CellBelow().SetValue(1)
                         .CellBelow().SetValue(4);

            ws.Cell("C1").SetValue("Strings")
             .CellBelow().SetValue("B")
             .CellBelow().SetValue("C")
             .CellBelow().SetValue("C")
             .CellBelow().SetValue("E")
             .CellBelow().SetValue("A")
             .CellBelow().SetValue("D");

            // Add filters
            ws.RangeUsed().SetAutoFilter().Column(2).BelowAverage();
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Guid.NewGuid().ToString()}_data.xlsx");
            }
            
        }

        [HttpGet("testauthdata")]
        [Authorize]
      //  [Authorize(Roles =("Admin"))]
        //[AuthorizeMultiple(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<IEnumerable<string>> GetTestDat()
        {
            TokenUserData tokenUserData = _hTTPUserRepository.LogCurrentUser();
            return new string[] { "value1", "value2" };
        }

        [HttpPost("bulkroom")]

        public async Task<ActionResult> SaveMutiRoom(List<string> request, CancellationToken cancellationToken)
        {

            foreach (var item in request)
            {
                var CampId = GetRandomData(1, 6);
                var BedCount = GetRandomData(2, 6);
                var Number = item;
                var Private = GetRandomData(0, 1);
                var VirtualRoom = 0;
                var RoomTypeId = GetRandomData(1, 10);
                
                await _mediator.Send(new CreateRoomRequest(Number, BedCount, Private, CampId, RoomTypeId, VirtualRoom, 1), cancellationToken);
            }
            

            return Ok();
        }

        private int GetRandomData(int minValue, int maxValue)
        {
            Random rnd = new Random();
            return rnd.Next(minValue, maxValue);
        }







    }




    public class MyModel
    {
        public string Name { get; set; }

        public IFormFile File { get; set; }
    }
}
