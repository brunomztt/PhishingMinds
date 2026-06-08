using Dapper;
using Microsoft.AspNetCore.Mvc;
using PhishingMinds.Server.Data;

namespace PhishingMinds.Server.Controllers
{
    [ApiController]
    [Route("track")]
    public class TrackingController : ControllerBase
    {
        private readonly DbConnectionFactory _dbFactory;

        public TrackingController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        
        [HttpGet]
        public IActionResult Track([FromQuery] int idTarget)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                var target = db.QueryFirstOrDefault(@"
                    SELECT
                        IdUser,
                        LinkClicked
                    FROM PhishingCampaignTarget
                    WHERE IdTarget = @IdTarget
                ", new { IdTarget = idTarget });

                if (target == null)
                    return NotFound();

                // Só desconta na primeira vez
                if (!(bool)target.LinkClicked)
                {
                    db.Execute(@"
                        UPDATE Pessoa
                        SET PhishingScore =
                            GREATEST(0, PhishingScore - 40)
                        WHERE IdUser = @IdUser
                    ", new
                    {
                        IdUser = target.IdUser
                    });
                }

                db.Execute(@"
                    UPDATE PhishingCampaignTarget
                    SET
                        LinkClicked = 1,
                        Dt_Click = NOW()
                    WHERE IdTarget = @IdTarget
                ", new { IdTarget = idTarget });

                return Redirect("https://www.microsoft.com");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("open")]
        public IActionResult Open([FromQuery] int idTarget)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                var target = db.QueryFirstOrDefault(@"
                    SELECT
                        IdUser,
                        MailOpened
                    FROM PhishingCampaignTarget
                    WHERE IdTarget = @IdTarget
                ", new { IdTarget = idTarget });

                if (target == null)
                    return NotFound();

                // Só desconta na primeira abertura
                if (!(bool)target.MailOpened)
                {
                    db.Execute(@"
                        UPDATE Pessoa
                        SET PhishingScore =
                            GREATEST(0, PhishingScore - 10)
                        WHERE IdUser = @IdUser
                    ", new
                    {
                        IdUser = target.IdUser
                    });
                }

                db.Execute(@"
                    UPDATE PhishingCampaignTarget
                    SET MailOpened = 1
                    WHERE IdTarget = @IdTarget
                ", new { IdTarget = idTarget });
                // imagem transparente 1x1 pixel
                byte[] pixel = Convert.FromBase64String(
                    "R0lGODlhAQABAPAAAP///wAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw=="
                );

                return File(pixel, "image/gif");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}