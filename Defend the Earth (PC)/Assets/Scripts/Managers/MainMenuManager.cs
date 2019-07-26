﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Spaceships Menu")]
    [SerializeField] private GameObject[] pages = new GameObject[0];

    //Buy Buttons
    [SerializeField] private Text buySpaceFighterButton = null;
    [SerializeField] private Text buyAlienMowerButton = null;
    [SerializeField] private Text buyBlazingRocketButton = null;
    [SerializeField] private Text buyQuadShooterButton = null;
    [SerializeField] private Text buyPointVoidBreakerButton = null;
    [SerializeField] private Text buyAnnihilatorButton = null;

    //Price Text
    [SerializeField] private Text spaceFighterPrice = null;
    [SerializeField] private Text alienMowerPrice = null;
    [SerializeField] private Text blazingRocketPrice = null;
    [SerializeField] private Text quadShooterPrice = null;
    [SerializeField] private Text pointVoidBreakerPrice = null;
    [SerializeField] private Text annihilatorPrice = null;

    [Header("Upgrades Menu")]
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text damageText = null;
    [SerializeField] private Text fireRateText = null;
    [SerializeField] private Text upgradeDamageButton = null;
    [SerializeField] private Text upgradeSpeedButton = null;
    [SerializeField] private Text upgradeHealthButton = null;
    [SerializeField] private Text upgradeMoneyButton = null;
    [SerializeField] private Text speedText = null;
    [SerializeField] private Text healthText = null;
    [SerializeField] private Text damagePrice = null;
    [SerializeField] private Text speedPrice = null;
    [SerializeField] private Text healthPrice = null;
    [SerializeField] private Text moneyPrice = null;
    
    [Header("Sound Menu")]
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private Slider musicSlider = null;

    [Header("Setup")]
    [SerializeField] private Canvas mainMenu = null;
    [SerializeField] private Canvas shopMenu = null;
    [SerializeField] private Canvas upgradesMenu = null;
    [SerializeField] private Canvas spaceshipsMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas graphicsQualityMenu = null;
    [SerializeField] private Canvas soundMenu = null;
    [SerializeField] private Canvas selectDifficultyMenu = null;
    [SerializeField] private GameObject loadingText = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip cannotAfford = null;

    private AudioSource audioSource;
    private int page = 1;
    private bool pressedBumper = false;
    private bool loading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Time.timeScale = 1;
        AudioListener.pause = false;
        PlayerPrefs.DeleteKey("Difficulty");
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        PlayerPrefs.Save();
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        mainMenu.enabled = true;
        shopMenu.enabled = false;
        spaceshipsMenu.enabled = false;
        upgradesMenu.enabled = false;
        settingsMenu.enabled = false;
        graphicsQualityMenu.enabled = false;
        soundMenu.enabled = false;
        selectDifficultyMenu.enabled = false;
        foreach (GameObject page in pages)
        {
            if (page) page.SetActive(false);
        }
        if (pages[0])
        {
            pages[0].SetActive(true);
            page = 1;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11)) Screen.fullScreen = !Screen.fullScreen;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (shopMenu.enabled)
            {
                clickShop(false);
            } else if (spaceshipsMenu.enabled)
            {
                clickSpaceships(false);
            } else if (upgradesMenu.enabled)
            {
                clickUpgrades(false);
            } else if (settingsMenu.enabled)
            {
                clickSettings(false);
            } else if (graphicsQualityMenu.enabled)
            {
                clickGraphicsQuality(false);
            } else if (soundMenu.enabled)
            {
                clickSound(false);
            } else if (selectDifficultyMenu.enabled)
            {
                clickPlayGame(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            pressedBumper = true;
            changeSpaceshipsPage(false);
            pressedBumper = false;
        } else if (Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            pressedBumper = true;
            changeSpaceshipsPage(true);
            pressedBumper = false;
        }
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);

        //Updates volume data to match the slider values
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();

        if (PlayerPrefs.GetString("Money") != "")
        {
            moneyCount.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            moneyCount.text = "$0";
        }

        //Updates the money counter and upgrade price text
        damageText.text = "+" + PlayerPrefs.GetInt("DamagePercentage") + "% Damage";
        speedText.text = "+" + PlayerPrefs.GetInt("SpeedPercentage") + "% Speed";
        healthText.text = "+" + PlayerPrefs.GetInt("HealthPercentage") + "% Health";
        fireRateText.text = "+" + PlayerPrefs.GetInt("MoneyPercentage") + "% Money";

        //Sets the states of spaceship buy buttons
        spaceshipButtonState(buySpaceFighterButton, "SpaceFighter");
        spaceshipButtonState(buyAlienMowerButton, "AlienMower");
        spaceshipButtonState(buyBlazingRocketButton, "BlazingRocket");
        spaceshipButtonState(buyQuadShooterButton, "QuadShooter");
        spaceshipButtonState(buyPointVoidBreakerButton, "PointVoidBreaker");
        spaceshipButtonState(buyAnnihilatorButton, "Annihilator");

        //Sets the states of spaceship price text
        priceTextState(spaceFighterPrice, buySpaceFighterButton, false, false, false, "HasSpaceFighter", "", 0, 1, false);
        priceTextState(alienMowerPrice, buyAlienMowerButton, false, false, false, "HasAlienMower", "", 300, 1, false);
        priceTextState(blazingRocketPrice, buyBlazingRocketButton, false, false, false, "HasBlazingRocket", "", 950, 1, false);
        priceTextState(quadShooterPrice, buyQuadShooterButton, false, false, false, "HasQuadShooter", "", 1500, 1, false);
        priceTextState(pointVoidBreakerPrice, buyPointVoidBreakerButton, false, false, false, "HasPointVoidBreaker", "", 2300, 1, false);
        priceTextState(annihilatorPrice, buyAnnihilatorButton, false, false, false, "HasAnnihilator", "", 5000, 1, false);

        //Sets the states of upgrade price text
        priceTextState(damagePrice, upgradeDamageButton, true, true, true, "DamagePercentage", "DamagePrice", 8, 50, false);
        priceTextState(speedPrice, upgradeSpeedButton, true, true, true, "SpeedPercentage", "SpeedPrice", 5, 20, false);
        priceTextState(healthPrice, upgradeHealthButton, true, true, true, "HealthPercentage", "HealthPrice", 7, 100, false);
        priceTextState(moneyPrice, upgradeMoneyButton, true, true, true, "MoneyPercentage", "MoneyPrice", 4, 300, false);

        if (!loading)
        {
            loadingText.SetActive(false);
            moneyCount.gameObject.SetActive(true);
        } else
        {
            loadingText.SetActive(true);
            moneyCount.gameObject.SetActive(false);
        }

        //Checks if the player upgrades are above maximum values
        if (PlayerPrefs.GetFloat("DamageMultiplier") > 1.5f)
        {
            PlayerPrefs.SetFloat("DamageMultiplier", 1.5f);
            PlayerPrefs.SetInt("DamagePercentage", 50);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("SpeedMultiplier") > 1.2f)
        {
            PlayerPrefs.SetFloat("SpeedMultiplier", 1.2f);
            PlayerPrefs.SetInt("SpeedPercentage", 20);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("HealthMultiplier") > 2f)
        {
            PlayerPrefs.SetFloat("HealthMultiplier", 2f);
            PlayerPrefs.SetInt("HealthPercentage", 100);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("MoneyMultiplier") > 4)
        {
            PlayerPrefs.SetFloat("MoneyMultiplier", 4);
            PlayerPrefs.SetInt("MoneyPercentage", 300);
            PlayerPrefs.Save();
        }

        //Checks if money is below 0
        if (PlayerPrefs.GetInt("Money") < 0)
        {
            PlayerPrefs.SetInt("Money", 0);
            PlayerPrefs.Save();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
    }

    public void clickPlayGame(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!selectDifficultyMenu.enabled)
        {
            selectDifficultyMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            selectDifficultyMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickShop(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!shopMenu.enabled)
        {
            shopMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            shopMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickSettings(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!settingsMenu.enabled)
        {
            settingsMenu.enabled = true;
            mainMenu.enabled = false;
        } else
        {
            settingsMenu.enabled = false;
            mainMenu.enabled = true;
        }
    }

    public void clickQuitGame()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        Application.Quit();
    }

    public void clickSpaceships(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!spaceshipsMenu.enabled)
        {
            spaceshipsMenu.enabled = true;
            shopMenu.enabled = false;
        } else
        {
            spaceshipsMenu.enabled = false;
            shopMenu.enabled = true;
            foreach (GameObject page in pages)
            {
                if (page) page.SetActive(false);
            }
            if (pages[0])
            {
                pages[0].SetActive(true);
                page = 1;
            } else
            {
                spaceshipsMenu.enabled = false;
                shopMenu.enabled = true;
                Debug.LogError("Could not find page objects in the pages array!");
            }
        }
    }

    public void clickUpgrades(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!upgradesMenu.enabled)
        {
            upgradesMenu.enabled = true;
            shopMenu.enabled = false;
        } else
        {
            upgradesMenu.enabled = false;
            shopMenu.enabled = true;
        }
    }

    public void clickGraphicsQuality(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!graphicsQualityMenu.enabled)
        {
            graphicsQualityMenu.enabled = true;
            settingsMenu.enabled = false;
        } else
        {
            graphicsQualityMenu.enabled = false;
            settingsMenu.enabled = true;
        }
    }

    public void clickSound(bool clicked)
    {
        if (clicked && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (!soundMenu.enabled)
        {
            soundMenu.enabled = true;
            settingsMenu.enabled = false;
        } else
        {
            soundMenu.enabled = false;
            settingsMenu.enabled = true;
        }
    }

    public void startGame(int difficulty)
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            }
            else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (PlayerPrefs.GetInt("Level") > 0)
        {
            PlayerPrefs.SetInt("Difficulty", difficulty);
            if (PlayerPrefs.GetInt("Difficulty") < 1)
            {
                PlayerPrefs.SetInt("Difficulty", 1);
            } else if (PlayerPrefs.GetInt("Difficulty") > 4)
            {
                PlayerPrefs.SetInt("Difficulty", 4);
            }
            PlayerPrefs.Save();
            StartCoroutine(loadScene("Level " + PlayerPrefs.GetInt("Level")));
        } else
        {
            PlayerPrefs.SetInt("Difficulty", difficulty);
            PlayerPrefs.Save();
            StartCoroutine(loadScene("Level 1"));
        }
    }

    public void changeSpaceshipsPage(bool next)
    {
        if (!pressedBumper && audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        if (next)
        {
            ++page;
            if (page < pages.Length)
            {
                foreach (GameObject page in pages)
                {
                    if (page) page.SetActive(false);
                }
                pages[page - 1].SetActive(true);
            } else
            {
                foreach (GameObject page in pages)
                {
                    if (page) page.SetActive(false);
                }
                pages[pages.Length - 1].SetActive(true);
                page = pages.Length;
            }
        } else
        {
            --page;
            if (page > 0)
            {
                foreach (GameObject page in pages)
                {
                    if (page) page.SetActive(false);
                }
                pages[page - 1].SetActive(true);
            } else
            {
                foreach (GameObject page in pages)
                {
                    if (page) page.SetActive(false);
                }
                pages[0].SetActive(true);
                page = 1;
            }
        }
    }

    public void buySpaceFighter()
    {
        if (PlayerPrefs.GetInt("HasSpaceFighter") <= 0)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            PlayerPrefs.SetInt("HasSpaceFighter", 1);
            PlayerPrefs.Save();
        }
    }

    public void buyAlienMower()
    {
        if (PlayerPrefs.GetInt("HasAlienMower") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 300)
            {
                money -= 300;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasAlienMower", 1);
                PlayerPrefs.Save();
            } else if (money < 300)
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void buyBlazingRocket()
    {
        if (PlayerPrefs.GetInt("HasBlazingRocket") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 950)
            {
                money -= 950;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasBlazingRocket", 1);
                PlayerPrefs.Save();
            } else if (money < 950)
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void buyQuadShooter()
    {
        if (PlayerPrefs.GetInt("HasQuadShooter") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= 1500)
            {
                money -= 1500;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasQuadShooter", 1);
                PlayerPrefs.Save();
            } else if (money < 1500)
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void buyPointVoidBreaker()
    {
        if (PlayerPrefs.GetInt("HasPointVoidBreaker") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (audioSource)
            {
                if (money < 2300)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                } else
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
            if (money >= 2300)
            {
                money -= 2300;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasPointVoidBreaker", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void buyAnnihilator()
    {
        if (PlayerPrefs.GetInt("HasAnnihilator") <= 0)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (audioSource)
            {
                if (money < 5000)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                } else
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
            if (money >= 5000)
            {
                money -= 5000;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HasAnnihilator", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void equipSpaceship(string spaceship)
    {
        if (spaceship != "")
        {
            if (PlayerPrefs.HasKey("Has" + spaceship))
            {
                if (PlayerPrefs.GetInt("Has" + spaceship) >= 1)
                {
                    if (audioSource)
                    {
                        if (buttonClick)
                        {
                            audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                        } else
                        {
                            audioSource.volume = getVolumeData(true);
                            audioSource.Play();
                        }
                    }
                    PlayerPrefs.SetString("Spaceship", spaceship);
                }
            } else
            {
                if (PlayerPrefs.GetString("Spaceship") != "SpaceFighter") PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            }
            PlayerPrefs.Save();
       }
    }
     
    public void upgradeDamage()
    {
        if (PlayerPrefs.GetInt("DamagePercentage") < 50)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("DamagePrice"))
            {
                if (audioSource)
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
                money -= PlayerPrefs.GetInt("DamagePrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("DamagePrice", (int)(PlayerPrefs.GetInt("DamagePrice") * 1.7f));
                PlayerPrefs.SetFloat("DamageMultiplier", PlayerPrefs.GetFloat("DamageMultiplier") + 0.05f);
                PlayerPrefs.SetInt("DamagePercentage", PlayerPrefs.GetInt("DamagePercentage") + 5);
                PlayerPrefs.Save();
            } else if (money < PlayerPrefs.GetInt("DamagePrice"))
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void upgradeSpeed()
    {
        if (PlayerPrefs.GetInt("SpeedPercentage") < 20)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("SpeedPrice"))
            {
                if (audioSource)
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
                money -= PlayerPrefs.GetInt("SpeedPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("SpeedPrice", (int)(PlayerPrefs.GetInt("SpeedPrice") * 1.35f));
                PlayerPrefs.SetFloat("SpeedMultiplier", PlayerPrefs.GetFloat("SpeedMultiplier") + 0.01f);
                PlayerPrefs.SetInt("SpeedPercentage", PlayerPrefs.GetInt("SpeedPercentage") + 1);
                PlayerPrefs.Save();
            } else if (money < PlayerPrefs.GetInt("SpeedPrice"))
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void upgradeHealth()
    {
        if (PlayerPrefs.GetInt("HealthPercentage") < 100)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("HealthPrice"))
            {
                if (audioSource)
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
                money -= PlayerPrefs.GetInt("HealthPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("HealthPrice", (int)(PlayerPrefs.GetInt("HealthPrice") * 1.4f));
                PlayerPrefs.SetFloat("HealthMultiplier", PlayerPrefs.GetFloat("HealthMultiplier") + 0.05f);
                PlayerPrefs.SetInt("HealthPercentage", PlayerPrefs.GetInt("HealthPercentage") + 5);
                PlayerPrefs.Save();
            } else if (money < PlayerPrefs.GetInt("HealthPrice"))
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void upgradeMoney()
    {
        if (PlayerPrefs.GetInt("MoneyPercentage") < 300)
        {
            long money = long.Parse(PlayerPrefs.GetString("Money"));
            if (money >= PlayerPrefs.GetInt("MoneyPrice"))
            {
                if (audioSource)
                {
                    if (buttonClick)
                    {
                        audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
                money -= PlayerPrefs.GetInt("MoneyPrice");
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.SetInt("MoneyPrice", (int)(PlayerPrefs.GetInt("MoneyPrice") * 1.3f));
                PlayerPrefs.SetFloat("MoneyMultiplier", PlayerPrefs.GetFloat("MoneyMultiplier") + 0.1f);
                PlayerPrefs.SetInt("MoneyPercentage", PlayerPrefs.GetInt("MoneyPercentage") + 10);
                PlayerPrefs.Save();
            } else if (money < PlayerPrefs.GetInt("MoneyPrice"))
            {
                if (audioSource)
                {
                    if (cannotAfford)
                    {
                        audioSource.PlayOneShot(cannotAfford, getVolumeData(true));
                    } else
                    {
                        audioSource.volume = getVolumeData(true);
                        audioSource.Play();
                    }
                }
            }
        }
    }

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                loadingSlider.value = load.progress;
                loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                mainMenu.enabled = false;
                shopMenu.enabled = false;
                spaceshipsMenu.enabled = false;
                upgradesMenu.enabled = false;
                settingsMenu.enabled = false;
                graphicsQualityMenu.enabled = false;
                soundMenu.enabled = false;
                selectDifficultyMenu.enabled = false;
                yield return null;
            }
            loading = false;
            loadingSlider.value = 0;
            loadingPercentage.text = "0%";
        } else
        {
            StopCoroutine(loadScene(scene));
        }
    }

    void spaceshipButtonState(Text button, string spaceship)
    {
        if (button && spaceship != "")
        {
            if (PlayerPrefs.GetInt("Has" + spaceship) <= 0)
            {
                button.text = "Buy";
                button.rectTransform.sizeDelta = new Vector2(58, 41);
            } else
            {
                if (PlayerPrefs.GetString("Spaceship") != spaceship)
                {
                    button.text = "Use";
                    button.rectTransform.sizeDelta = new Vector2(46, 41);
                } else if (PlayerPrefs.GetString("Spaceship") == spaceship)
                {
                    button.text = "Using";
                    button.rectTransform.sizeDelta = new Vector2(68, 41);
                }
            }
        }
    }

    void priceTextState(Text main, Text button, bool isUpgrade, bool change, bool useDataKey, string statKey, string priceKey, int price, int max, bool isFloat)
    {
        if (main && statKey != "")
        {
            if (!isFloat)
            {
                if (PlayerPrefs.GetInt(statKey) < max)
                {
                    if (useDataKey)
                    {
                        if (PlayerPrefs.GetInt(priceKey) > 0)
                        {
                            main.text = "$" + PlayerPrefs.GetInt(priceKey);
                        } else
                        {
                            main.text = "Free";
                        }
                    } else
                    {
                        if (price > 0)
                        {
                            main.text = "$" + price;
                        } else
                        {
                            main.text = "Free";
                        }
                    }
                    main.color = new Color32(133, 187, 101, 255);
                    main.GetComponent<Outline>().effectColor = new Color32(67, 94, 50, 255);
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = new Vector2(100, 41);
                        button.text = "Upgrade";
                    }
                } else if (PlayerPrefs.GetInt(statKey) >= max)
                {
                    if (isUpgrade)
                    {
                        main.text = "Maxed out!";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    } else
                    {
                        main.text = "Owned";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    }
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = Vector2.zero;
                        button.text = "";
                    }
                }
            } else
            {
                if (PlayerPrefs.GetFloat(statKey) < max)
                {
                    if (PlayerPrefs.GetInt(priceKey) > 0)
                    {
                        main.text = "$" + PlayerPrefs.GetInt(priceKey);
                    } else
                    {
                        main.text = "Free";
                    }
                    main.color = new Color32(133, 187, 101, 255);
                    main.GetComponent<Outline>().effectColor = new Color32(67, 94, 50, 255);
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = new Vector2(100, 41);
                        button.text = "Upgrade";
                    }
                }
                else if (PlayerPrefs.GetFloat(statKey) >= max)
                {
                    if (isUpgrade)
                    {
                        main.text = "Maxed out!";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    } else
                    {
                        main.text = "Owned";
                        main.color = new Color32(255, 215, 0, 255);
                        main.GetComponent<Outline>().effectColor = new Color32(127, 107, 0, 255);
                    }
                    if (button && change)
                    {
                        button.rectTransform.sizeDelta = Vector2.zero;
                        button.text = "";
                    }
                }
            }
        }
    }
}