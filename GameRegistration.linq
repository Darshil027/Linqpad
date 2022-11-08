<Query Kind="Program">
  <Connection>
    <ID>446b2ebd-274c-4d7f-b298-b51ad03b06ac</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>LP710</Server>
    <Database>FSIS_2018</Database>
    <DisplayName>FSIS_2018-Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	LoadTeams();
	LoadGames();
	
	DateTime GDate = new DateTime(2008, 5, 1, 8, 30, 52);
	int HTeamID = 16;
	int HTeamScore = 3;
	int VTeamID = 17;
	int VTeamScore = 2;

	GameStat gplay = new GameStat();
		gplay.GameDate = GDate;
		gplay.HomeTeamId = HTeamID;
		gplay.HomeTeamScore = HTeamScore;
		gplay.VisitingTeamId = VTeamID;
		gplay.VisitingTeamScore = VTeamScore;

	Game_RecordGame(gplay);
		
}

public class GameStat
{
	public DateTime GameDate { get; set; }
	public int HomeTeamId { get; set; }
	public int HomeTeamScore { get; set; }
	public int VisitingTeamId { get; set; }
	public int VisitingTeamScore { get; set; }
}


public void LoadTeams()
{
	var gs = Teams
				.Select(t => new
				{
					HomeTeamId = t.TeamID,
					TeamName = t.TeamName,
					Wins = t.Wins,
					Losses = t.Losses
				});
	//.Dump();
}

public void LoadGames()
{
	var lg = Games
				.Select(g => new
				{
					ID = g.GameID,
					Date = g.GameDate,
					HomeTeamId = g.HomeTeamID,
					HomeTeamName = g.Home.TeamName,
					HomeTeamScore = g.HomeTeamScore,
					VisitingTeamId = g.VisitingTeamID,
					VisitingTeamName = g.Visiting.TeamName,
					VisitingTeamScore = g.VisitingTeamScore
				}).OrderByDescending(g => g.Date)
				.Dump();

}

public void Game_RecordGame(GameStat item)
{
	List<Exception> errorList = new List<Exception>();

	if (item.HomeTeamId == item.VisitingTeamId)
	{
		errorList.Add(new ArgumentException("HomeTeamID and VisitingTeamID cannot be the same. "));
	}
	if (item.HomeTeamScore ==  item.VisitingTeamScore)
	{
		errorList.Add(new ArgumentException("Game cannot be Tied."));
	}
	if (item.GameDate > DateTime.Now)
	{
		errorList.Add(new ArgumentException("Game Time cannot be future time."));
	}
	
	bool HTeamExists = false;
		
	HTeamExists = Teams
					.Where(t => t.TeamID == item.HomeTeamId)
					.Any();
	
	if (HTeamExists)
	{
		errorList.Add(new ArgumentException("Home Team Does not Exists."));	
	}
	
	bool VTeamExists = false;
	VTeamExists = Teams
					.Where(t => t.TeamID == item.VisitingTeamId)
					.Any();
	if (VTeamExists)
	{
		errorList.Add(new ArgumentException("Visiting Team Does not Exists."));
	}
	
	bool ExistingGame = false;
	
	ExistingGame = Games
					.Where(x => x.HomeTeamID == item.HomeTeamId
					&& x.VisitingTeamID == item.VisitingTeamId
					&& x.GameDate == item.GameDate)
					.Any();
	if (ExistingGame)
	{
		errorList.Add(new ArgumentException("Game already exist on the same date and teams."));
	}
	
	foreach (GameStat g in gameTRX)
	{
		Teams ts = Teams
					.Where(es => es.TeamID == ts.TeamID &&
											   es.SkillID == s.SkillId)
										.FirstOrDefault();
	}
	
	
}





