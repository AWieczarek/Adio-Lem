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
    [SerializeField] private Button m_connectBtn = null;
    [SerializeField] private TextMeshProUGUI m_artistText = null;
    [SerializeField] private TextMeshProUGUI m_trackText = null;
    [SerializeField] private TextMeshProUGUI m_albumText = null;
    [SerializeField] private Button m_previousBtn = null;
    [SerializeField] private Button m_nextBtn = null;
    [SerializeField] private Button m_playBtn = null;
    [SerializeField] private Button m_pauseBtn = null;
    [SerializeField] private Image m_albumArt = null;
    [SerializeField] private Sprite[] m_shuffleSprites = null;
    [SerializeField] private Sprite AdvertSprite = null;
    [SerializeField] private Button m_shuffleBtn = null;

    [SerializeField] private Spotify4Unity.Enums.Resolution m_albumArtResolution = Spotify4Unity.Enums.Resolution.Original;
    [SerializeField] private Color m_errorColor = new Color(1f, 0f, 0f, 0.5f);


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

    protected override void OnPlayStatusChanged(PlayStatusChanged e)
    {
        base.OnPlayStatusChanged(e);

        // If a Play btn & Pause btn is configured, set it's correct displaying state
        if (m_playBtn != null && m_playBtn.isActiveAndEnabled != !e.IsPlaying)
        {
            m_playBtn.gameObject.SetActive(!e.IsPlaying);
        }
        if (m_pauseBtn != null && m_pauseBtn.isActiveAndEnabled != e.IsPlaying)
        {
            m_pauseBtn.gameObject.SetActive(e.IsPlaying);
        }
    }

    protected override void OnTrackChanged(TrackChanged e)
    {
        if (e != null)
        {
            // Load the Album Art for the new Track
            LoadAlbumArt(e.NewTrack, m_albumArtResolution);
            SetTrackInfo(e.NewTrack.Title, e.NewTrack.Album, String.Join(", ", e.NewTrack.Artists.Select(x => x.Name)));

        }
    }

    private void LoadAlbumArt(Track t, Spotify4Unity.Enums.Resolution resolution)
    {
        // Get the URL and load on a Coroutine
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
        if (m_albumArt != null)
        {
            m_albumArt.sprite = s;
        }
    }

    public async void OnClickShuffle()
    {
        Shuffle state = SpotifyService.ShuffleState;

        if (state == 0)
        {
            state = (Shuffle)1;
            m_shuffleBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            state = (Shuffle)0;
            m_shuffleBtn.GetComponent<Image>().color = Color.grey;
        }

        await SpotifyService.SetShuffleAsync(state);
    }


    protected override void OnMediaTypeChanged(MediaTypeChanged e)
    {
        if (e.MediaType == MediaType.Advert)
        {
            if (m_albumArt != null)
                m_albumArt.sprite = AdvertSprite;

            SetTrackInfo("Advert", "Unknown", "Unknown");
        }
    }

    private void SetBtnPressedTint(ref Button btn)
    {
        if (btn == null)
            return;
        ColorBlock colors = btn.colors;
        colors.pressedColor = m_errorColor;
        btn.colors = colors;
    }

    protected void SetSprite(int stateIndex, Sprite[] spritesArray, Image image, string errorMsg)
    {
        if (stateIndex >= spritesArray.Length)
        {
            Analysis.LogError(errorMsg, Analysis.LogLevel.All);
            return;
        }

        if (image != null)
            image.sprite = spritesArray[stateIndex];
    }

    private void SetTrackInfo(string track, string artist, string album)
    {
        if (m_artistText != null)
            m_artistText.text = artist;

        if (m_trackText != null)
            m_trackText.text = track;

        if (m_albumText != null)
            m_albumText.text = album;
    }

    private void OnDisconnect()
    {
        SpotifyService.Disconnect();
    }
}
