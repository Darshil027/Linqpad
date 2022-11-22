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

}

public class PlayerGameStat

{
	public int PlayerID { get; set; }
	public int Goals { get; set; }
	public int Assist { get; set; }
	public bool YellowCard { get; set; }
	public bool RedCard { get; set; }
	public bool Act { get; set; }
}

void RecordGamePlayerStats(int gameId, List<PlayerGameStat> hometeam, List<PlayerGameStat> visitingteam)
{
	List<Exception> errorList = new List<Exception>();
	// Validation 1 - parameter values must exist (ArgumentNullException).
	if (string.IsNullOrEmpty(gameId.ToString()))
	{
		throw new ArgumentNullException("Game ID is missing ");
	}
	if (hometeam == null)
	{
		throw new ArgumentNullException("Home Team is missing ");
	}
	if (visitingteam == null)
	{
		throw new ArgumentNullException("Visitng Team is Missing ");
	}

	bool gameExists = Games
				.Where(x => x.GameID == gameId).Any();

	if (!gameExists)
	{
		throw new ArgumentException($"The Game with game ID:{gameId} does not exist on the database!");
	}

	// Validation 2 -The Games record for the incoming stats should already be on the database (ArgumentException).
	// Validation 3 - player stats for the game must not already be on file 
	// Both conditions must be true 
	bool IsGameRecordExist = Games
		.Where(x => x.PlayerStats.Count() == 0 && x.GameID == gameId)
		.Any();
	if (!IsGameRecordExist)
	{
		errorList.Add(new ArgumentException($"The Game does not exist on the database, or it has already stats registered"));
	}

	//Validation 4 - goals and assists are non-negative integer values, 
	// Validation for Team 1 
	foreach (var eachHomeStats in hometeam)
	{
		if (eachHomeStats.Goals < 0)
		{
			errorList.Add(new ArgumentException("Home Team Goals Cannot be in negative"));
		}
		if (eachHomeStats.Assist < 0)
		{
			errorList.Add(new ArgumentException("Home Team Assists Cannot be in negative"));
		}
	}
	// Validation for Team 2 
	foreach (var eachOppStats in visitingteam)
	{
		if (eachOppStats.Goals < 0)
		{
			errorList.Add(new ArgumentException("Opponent Team Goals Cannot be in negative"));
		}
		if (eachOppStats.Assist < 0)
		{
			errorList.Add(new ArgumentException("Opponent Team Assists Cannot be in negative"));
		}
	}

	//Validaton 5 - total number of goals by players on a team is equal to the team score recorded for the game.
	//set the end goals to zero and then add 
	int goalsHomeTeam = 0;
	int goalsOppTeam = 0;
	foreach (var eachHomeStats in hometeam)
	{
		goalsHomeTeam = goalsHomeTeam + eachHomeStats.Goals;
	}


	foreach (var eachOppStats in visitingteam)
	{
		goalsOppTeam = goalsOppTeam + eachOppStats.Goals;
	}


	if (goalsHomeTeam != Games
						.Where(x => x.GameID == gameId)
						.Select(x => x.HomeTeamScore)
						.FirstOrDefault())

	{
		throw new Exception("The score scored by each player in the home team does not match with the combine team score");
	}

	if (goalsOppTeam != Games
						.Where(x => x.GameID == gameId)
						.Select(x => x.HomeTeamScore)
						.FirstOrDefault())

	{
		throw new Exception("The score scored by each player in the visiting does not match with the combine team score");
	}




}