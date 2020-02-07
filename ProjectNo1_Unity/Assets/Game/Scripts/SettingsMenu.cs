using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEditor;
using System;

public class SettingsMenu : MonoBehaviour
{
    //GameObjects - public fields
    public AudioMixer AudioMixerAM;
    public Dropdown ResolutionDrd;
    public Dropdown QualityDrd;
    public Slider AudioSl;
    public Toggle IsFullScreenTg;
    public GameObject ApplySettingsObj;
    public Text applyTmrTxt;
    //public Text resolutionttext;

    //private fields
    private Resolution[] _resolutions;
    private int _currentResolutionIndex;
    private int _resolutionIndex_temp;
    private float _volume_temp;
    private int _qualityIndex_temp;
    private bool _isFullscreen_temp;
    private float _targetTime = 3.0f;
    private bool _turnOnApplyTimer = false;

    void Start()
    {
        

        //Fill resolution dropdown options

        _resolutions = Screen.resolutions;
        _currentResolutionIndex = 0;
        List<string> resOptions = new List<string>();

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height;
            resOptions.Add(option);
            if (PlayerPrefs.GetInt("resolutionIndex") > 0)
            {
                _currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex");
            }
            else
            {
                if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
                {
                    _currentResolutionIndex = i;
                }
            }
        }
        ResolutionDrd.ClearOptions();
        ResolutionDrd.AddOptions(resOptions);
        ResolutionDrd.value = _currentResolutionIndex;
        ResolutionDrd.RefreshShownValue();

        //Read saved options and store in temp var

        _resolutionIndex_temp = ResolutionDrd.value;
        _qualityIndex_temp = QualitySettings.GetQualityLevel();
        _volume_temp = PlayerPrefs.GetFloat("volume");
        AudioMixerAM.SetFloat("volume", _volume_temp);
        _isFullscreen_temp = Screen.fullScreen;

        //Refresh choices

        ResolutionDrd.value = _resolutionIndex_temp;
        QualityDrd.value = _qualityIndex_temp;
        AudioSl.value = _volume_temp;
        IsFullScreenTg.isOn = _isFullscreen_temp;

        //hide unnecessary objects
        ApplySettingsObj.SetActive(false);
    }

    public void ApplyScreenSettings()
    {
        //if any change - apply

        if (ResolutionDrd.value != _resolutionIndex_temp ||
            QualityDrd.value != _qualityIndex_temp ||
            AudioSl.value != _volume_temp ||
            IsFullScreenTg.isOn != _isFullscreen_temp)
        {
            //aplly current settings
            setOptions();

            //timer info
            _targetTime = 3.0f;
            _turnOnApplyTimer = true;

            //show apply window
            ApplySettingsObj.SetActive(true);
        }
    }
    private void setOptions()
    {
        //resolution
        Resolution resolution = _resolutions[ResolutionDrd.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", ResolutionDrd.value);

        //volume
        AudioMixerAM.SetFloat("volume", AudioSl.value);
        PlayerPrefs.SetFloat("volume", AudioSl.value);

        //quality
        QualitySettings.SetQualityLevel(QualityDrd.value);

        //full screen
        Screen.fullScreen = IsFullScreenTg.isOn;
    }
    public void ApplyApply()
    {
        //if pressed apply within 3 sec

        _turnOnApplyTimer = false;
        _resolutionIndex_temp = ResolutionDrd.value;
        _qualityIndex_temp = QualityDrd.value;
        _volume_temp = AudioSl.value;
        _isFullscreen_temp = IsFullScreenTg.isOn;
        ApplySettingsObj.SetActive(false);
    }
    private void Update()
    {

        if (_turnOnApplyTimer)
        {
            _targetTime -= Time.deltaTime;
            if (_targetTime <= 0.0f)
            {
                //if 3 sec passed - reset settings to previous

                ResolutionDrd.value = _resolutionIndex_temp;
                QualityDrd.value = _qualityIndex_temp;
                IsFullScreenTg.isOn = _isFullscreen_temp;
                AudioSl.value = _volume_temp;

                setOptions();

                _turnOnApplyTimer = false;
                _targetTime = 3.0f;

                ApplySettingsObj.SetActive(false);
            }
            applyTmrTxt.text = Convert.ToInt32(_targetTime).ToString();
        }
        //resolutionttext.text = PlayerPrefs.GetInt("resolutionIndex").ToString();
    }
    public void CleanPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
}
