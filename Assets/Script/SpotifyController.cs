using Spotify4Unity;
using Spotify4Unity.Dtos;
using Spotify4Unity.Enums;
using Spotify4Unity.Events;
using Spotify4Unity.Helpers;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SpotifyController : SpotifyUIBase
{
    [SerializeField] private GameObject connectCanvas = null;
    [SerializeField] private GameObject connectingSpinner = null;

    [SerializeField] private TextMeshProUGUI trackText = null;
    [SerializeField] private TextMeshProUGUI albumText = null;

    [SerializeField] private TextMeshProUGUI recentSongTrackText = null;
    [SerializeField] private TextMeshProUGUI recentSongAlbumText = null;

    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button pauseBtn = null;

    [SerializeField] private Image albumArt = null;
    [SerializeField] private Image recentSongAlbumArt = null;

    [SerializeField] private Sprite advertSprite = null;
    [SerializeField] private Button shuffleBtn = null;

    [SerializeField] private Spotify4Unity.Enums.Resolution albumArtResolution = Spotify4Unity.Enums.Resolution.Original;

    public void OnConnect()
    {
        if (!SpotifyService.IsConnected)
        {
            connectCanvas.SetActive(false);
            connectingSpinner.SetActive(true);

            bool didAttempt = SpotifyService.Connect();
            if (!didAttempt)
            {
                connectCanvas.SetActive(true);
                connectingSpinner.SetActive(false);
            }
        }
    }

    public async void OnNextMedia()
    {
        await SpotifyService.NextSongAsync();
    }

    public async void OnPreviousMedia()
    {
        await SpotifyService.PreviousSongAsync();
    }

    public async void OnPauseMedia()
    {
        await SpotifyService.PauseAsync();
    }

    public async void OnPlayMedia()
    {
        await SpotifyService.PlayAsync();
    }

    protected override void OnConnectingChanged(ConnectingChanged e)
    {
        base.OnConnectingChanged(e);

        connectCanvas.SetActive(!e.IsConnecting && !SpotifyService.IsConnected);
        connectingSpinner.SetActive(e.IsConnecting && !SpotifyService.IsConnected);
    }

    protected override void OnPlayStatusChanged(PlayStatusChanged e)
    {
        base.OnPlayStatusChanged(e);

        if (playBtn != null && playBtn.isActiveAndEnabled != !e.IsPlaying)
        {
            playBtn.gameObject.SetActive(!e.IsPlaying);
        }
        if (pauseBtn != null && pauseBtn.isActiveAndEnabled != e.IsPlaying)
        {
            pauseBtn.gameObject.SetActive(e.IsPlaying);
        }
    }

    protected override void OnTrackChanged(TrackChanged e)
    {
        if (e != null)
        {
            LoadAlbumArt(e.NewTrack, albumArtResolution);
            SetTrackInfo(e.NewTrack.Title, String.Join(", ", e.NewTrack.Artists.Select(x => x.Name)), e.NewTrack.Album);
        }
    }

    private void LoadAlbumArt(Track t, Spotify4Unity.Enums.Resolution resolution)
    {
        string url = t.GetAlbumArtUrl();
        if (!string.IsNullOrEmpty(url))
        {
            if (this.isActiveAndEnabled)
                StartCoroutine(Utility.LoadImageFromUrl(url, resolution, sprite => OnAlbumArtLoaded(sprite)));
            else
                Utility.RunCoroutineEmptyObject(Utility.LoadImageFromUrl(url, resolution, sprite => OnAlbumArtLoaded(sprite)));
        }
    }

    private void OnAlbumArtLoaded(Sprite s)
    {
        if (albumArt != null)
        {
            albumArt.sprite = s;
            recentSongAlbumArt.sprite = s;
        }
    }

    public async void OnClickShuffle()
    {
        Shuffle state = SpotifyService.ShuffleState;

        if (state == 0)
        {
            state = (Shuffle)1;
            shuffleBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            state = (Shuffle)0;
            shuffleBtn.GetComponent<Image>().color = Color.grey;
        }

        await SpotifyService.SetShuffleAsync(state);
    }


    protected override void OnMediaTypeChanged(MediaTypeChanged e)
    {
        if (e.MediaType == MediaType.Advert)
        {
            if (albumArt != null)
                albumArt.sprite = advertSprite;

            SetTrackInfo("Advert", "Unknown", "Unknown");
        }
    }

    private void SetTrackInfo(string track, string artist, string album)
    {
        if (trackText != null)
        {
            trackText.text = artist + " - " + track;
            recentSongTrackText.text = artist + " - " + track;
        }
            

        if (albumText != null)
        {
            albumText.text = album;
            recentSongAlbumText.text = album;
        }
    }
}
