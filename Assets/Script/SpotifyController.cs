using SpotifyAPI.Web;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class SpotifyController : SpotifyPlayerListener
{
    [SerializeField]
    private Image _trackIcon;
    
    [SerializeField]
    private TMP_Text _trackName, _artistsNames;
    
    [SerializeField]
    private Button _playPauseButton, _previousButton, _nextButton, _shuffleButton;
    
    [SerializeField]
    private Sprite _playSprite, _pauseSprite;

    protected override void Awake()
    {
        base.Awake();

        this.OnPlayingItemChanged += this.PlayingItemChanged;
    }

    private void Start()
    {
        if (_playPauseButton != null)
        {
            _playPauseButton.onClick.AddListener(() => this.OnPlayPauseClicked());
        }
        if (_previousButton != null)
        {
            _previousButton.onClick.AddListener(() => this.OnPreviousClicked());
        }
        if (_nextButton != null)
        {
            _nextButton.onClick.AddListener(() => this.OnNextClicked());
        }
        if (_shuffleButton != null)
        {
            _shuffleButton.onClick.AddListener(() => this.OnToggleShuffle());
        }
    }
    

    private void Update()
    {
        CurrentlyPlayingContext context = GetCurrentContext();
        if (context != null)
        {
            bool currentShuffleState = GetCurrentContext().ShuffleState;
            _shuffleButton.transform.GetComponentInChildren<Image>().color = (currentShuffleState) ? Color.white : Color.grey;
            
            if (_playPauseButton != null)
            {
                Image playPauseImg = _playPauseButton.transform.GetComponentInChildren<Image>();
                if (context.IsPlaying)
                {
                    playPauseImg.sprite = _pauseSprite;
                }
                else
                {
                    playPauseImg.sprite = _playSprite;
                }
            }
        }
    }

    private async void PlayingItemChanged(IPlayableItem newPlayingItem)
    {
        if (newPlayingItem == null)
        {
            UpdatePlayerInfo("No track playing", "No track playing", "");
        }
        else
        {
            if (newPlayingItem.Type == ItemType.Track)
            {
                if (newPlayingItem is FullTrack track)
                {
                    string allArtists = S4UUtility.ArtistsToSeparatedString(", ", track.Artists);
                    SpotifyAPI.Web.Image image = S4UUtility.GetLowestResolutionImage(track.Album.Images);
                    UpdatePlayerInfo(track.Name, allArtists, image?.Url);
                }
            }
            else if (newPlayingItem.Type == ItemType.Episode)
            {
                if (newPlayingItem is FullEpisode episode)
                {
                    string creators = episode.Show.Publisher;
                    SpotifyAPI.Web.Image image = S4UUtility.GetLowestResolutionImage(episode.Images);
                    UpdatePlayerInfo(episode.Name, creators, image?.Url);
                }
            }
        }
    }

    private void UpdatePlayerInfo(string trackName, string artistNames, string artUrl)
    {
        if (_trackName != null)
        {
            _trackName.text = trackName;
        }
        if (_artistsNames != null)
        {
            _artistsNames.text = artistNames;
        }
        if (_trackIcon != null)
        {
            if (string.IsNullOrEmpty(artUrl))
            {
                _trackIcon.sprite = null;
            }
            else
            {
                StartCoroutine(S4UUtility.LoadImageFromUrl(artUrl, (loadedSprite) =>
                {
                    _trackIcon.sprite = loadedSprite;
                }));
            }
        }
    }
    
    private void OnPlayPauseClicked()
    {
        CurrentlyPlayingContext context = GetCurrentContext();
        SpotifyClient client = SpotifyService.Instance.GetSpotifyClient();
        if (context != null && client != null)
        {
            Image playPauseImg = _playPauseButton.transform.GetComponentInChildren<Image>();
            if (context.IsPlaying)
            {
                client.Player.PausePlayback();
                playPauseImg.sprite = _playSprite;
            }
            else
            {
                client.Player.ResumePlayback();
                playPauseImg.sprite = _pauseSprite;
            }
        }
    }

    private void OnPreviousClicked()
    {
        SpotifyClient client = SpotifyService.Instance.GetSpotifyClient();
        if(client != null)
        {
            client.Player.SkipPrevious();
        }
    }

    private void OnNextClicked()
    {
        SpotifyClient client = SpotifyService.Instance.GetSpotifyClient();
        if (client != null)
        {
            client.Player.SkipNext();
        }
    }

    private void OnToggleShuffle()
    {
        SpotifyClient client = SpotifyService.Instance.GetSpotifyClient();
        if (client != null)
        {
            bool currentShuffleState = GetCurrentContext().ShuffleState;

            _shuffleButton.transform.GetComponentInChildren<Image>().color = (currentShuffleState) ? Color.white : Color.grey;

            PlayerShuffleRequest request = new PlayerShuffleRequest(!currentShuffleState);

            client.Player.SetShuffle(request);
        }
    }
}
