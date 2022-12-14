<Query Kind="Program">
  <Connection>
    <ID>82822b70-fc6e-45b2-b173-3f93b66b5371</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>LP710</Server>
    <DisplayName>Chinook-Entity</DisplayName>
    <Database>Chinook2018</Database>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	try
	{
		//  Post
		#region Query
		//  query method using linq to entity
		//  Track_FetchTrackBy
		//  TrackService is our BLL class name
		//	FetchTrackBy is our method name

		string searchPattern = "deep";
		string searchType = "Artist";

		List<TrackSelection> trackList_display =
										TrackService_FetchTrackBy(searchType, searchPattern);
		//trackList_display.Dump();



		//	PlaylistTrackService is the BLL class name
		//  FetchPlaylist is the method call

		string playlistName = "hansenb1";
		string username = "HansenB";

		List<PlaylistTrackInfo> playlist_display =
				PlaylistTrackService_FetchPlaylist(playlistName, username);
		//playlist_display.Dump();
		#endregion

		#region Command method using Linq to Entity
		//	793 A Castle Full of Rascals
		//	822	A Twist In The Tail
		//	543	Burn
		//	756	Child in Time

		playlistName = "hansenb1";
		int trackID = 822;

		PlaylistTrackService_AddTrack(playlistName, username, trackID);

		#endregion
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}

	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}
// You can define other methods, fields, classes and namespaces here

#region Methods
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion

#region Models
public class TrackSelection
{
	public int TrackId { get; set; }
	public string SongName { get; set; }
	public string AlbumTitle { get; set; }
	public string ArtistName { get; set; }
	public int Milliseconds { get; set; }
	public decimal Price { get; set; }
}

public class PlaylistTrackInfo
{
	public int TrackId { get; set; }
	public int TrackNumber { get; set; }
	public string SongName { get; set; }
	public int Milliseconds { get; set; }
}
#endregion

#region TrackService Class
#region Query
public List<TrackSelection> TrackService_FetchTrackBy(string searchType,
												string searchPattern)
{
	IEnumerable<TrackSelection> tracks = Tracks
	.Where(x => searchType.Equals("Artist") ?
								x.Album.Artist.Name.Contains(searchPattern) :
								x.Album.Title.Contains(searchPattern))
				.Select(x => new TrackSelection
				{
					TrackId = x.TrackId,
					SongName = x.Name,
					AlbumTitle = x.Album.Title,
					ArtistName = x.Album.Artist.Name,
					Milliseconds = x.Milliseconds,
					Price = x.UnitPrice
				});

	return tracks.ToList();
}
#endregion
#endregion

#region PlaylistTrack Service
#region Query
public List<PlaylistTrackInfo> PlaylistTrackService_FetchPlaylist(string playlistName, string username)
{
	IEnumerable<PlaylistTrackInfo> playlist = PlaylistTracks
									.Where(x => x.Playlist.Name == playlistName
									&& x.Playlist.UserName == username)
									.Select(x => new PlaylistTrackInfo
									{
										TrackId = x.TrackId,
										TrackNumber = x.TrackNumber,
										SongName = x.Track.Name,
										Milliseconds = x.Track.Milliseconds
									})
									.OrderBy(x => x.TrackNumber);
	return playlist.ToList();
}
#endregion
#endregion

#region Commands

public void PlaylistTrackService_AddTrack(string playlistName, string username, int trackID)
{
	//  local variables
	bool trackExist = false;
	Playlists playlist = null;
	int trackNumber = 0;
	PlaylistTracks playlistTrackExist = null;

	#region Business Logic and Parameter Exceptions
	List<Exception> errorList = new List<Exception>();

	//  Business Rules
	//	There are processing rules that need to be satisfied for valid data.
	//		rule:	a track can only exist once on a playlist
	//		rule:	each track on a playlist is assigned a continous (sequential) track number
	//
	//	If the business rules are passed, consider the data valid, then
	//		a)	stage your transaction work (Adds, Updates or Deletes)
	//		b)	execute a SINGLE .SaveChanges() - commits to database

	//	We could assume that user name and track ID will always be valid (James)

	//  parameter validation
	if (string.IsNullOrWhiteSpace(playlistName))
	{
		throw new ArgumentNullException("Playlist name is missing");
	}
	if (string.IsNullOrWhiteSpace(username))
	{
		throw new ArgumentNullException("User name is missing");
	}
	#endregion
	//  check that the incoming data exists
	trackExist = Tracks
					.Where(x => x.TrackId == trackID)
					.Any();
	if (!trackExist)
	{
		throw new ArgumentNullException("Selected track no longer is on file.  Refresh track table");
	}

	//  Business Rules
	//  Check if playlist exists.
	playlist = Playlists
				.Where(x => x.Name == playlistName &&
						x.UserName == username)
						.FirstOrDefault();
	//  does not exist
	if (playlist == null)
	{
		playlist = new Playlists
		{
			Name = playlistName,
			UserName = username,
		};
		//  stage (only in memory)
		Playlists.Add(playlist);
		trackNumber = 1;
	}
	else
	{
		playlistTrackExist = PlaylistTracks
							.Where(x => x.TrackId == trackID &&
							x.PlaylistId == playlist.PlaylistId)
							.FirstOrDefault();
		if (playlistTrackExist != null)
		{
			var songName = Tracks
							.Where(x => x.TrackId == trackID)
							.Select(x => x.Name);
			errorList.Add(new Exception($"Selected track ({songName}) is already on the playlist"));
		}
		else
		{
			trackNumber = PlaylistTracks
							.Where(x => x.PlaylistId == playlist.PlaylistId)
							.Count() + 1;
		}
	}

}



#endregion