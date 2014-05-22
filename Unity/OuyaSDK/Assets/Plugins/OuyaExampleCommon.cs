﻿/*
 * Copyright (C) 2012, 2013 OUYA, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
#if UNITY_ANDROID && !UNITY_EDITOR
using tv.ouya.console.api;
#endif
using UnityEngine;

public class OuyaExampleCommon
{
    /// <summary>
    /// The current selected controller
    /// </summary>
    public static OuyaSDK.OuyaPlayer Player = OuyaSDK.OuyaPlayer.player1;

    #region Mapping Helpers

    public delegate float GetAxisDelegate(string axisName);

    public static float GetAxis(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
    {
        return GetAxisWithDelegate(keyCode, player, Input.GetAxis);
    }

    public static float GetAxisRaw(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
    {
        return GetAxisWithDelegate(keyCode, player, Input.GetAxisRaw);
    }

    public static float GetAxis(string ouyaMapping, OuyaSDK.OuyaPlayer player)
    {
        switch (ouyaMapping)
        {
            case "LB":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_LB, player);
            case "LT":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_LT, player);
            case "LX":
                return GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, player);
            case "LY":
                return GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, player);
            case "RB":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_RB, player);
            case "RT":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_RT, player);
            case "RX":
                return GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_X, player);
            case "RY":
                return GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_Y, player);
            case "DL":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, player);
            case "DR":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, player);
            case "DU":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_UP, player);
            case "DD":
                return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, player);
            default:
                return 0f;
        }
    }

    public static float GetAxisWithDelegate(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player, GetAxisDelegate action)
    {
        if (null == OuyaSDK.Joysticks)
        {
            return 0f;
        }
        
        int playerIndex = (int)player - 1;

#if UNITY_ANDROID && !UNITY_EDITOR
        switch (keyCode)
        {
            case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_LS_X);
            case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_LS_Y);
            case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_RS_X);
            case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_RS_Y);
            case OuyaSDK.KeyEnum.AXIS_LT:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_L2);
            case OuyaSDK.KeyEnum.AXIS_RT:
                return OuyaSDK.OuyaInput.GetAxis(playerIndex, OuyaController.AXIS_R2);
            default:
                return 0f;
        }

#else

        if (playerIndex >= OuyaSDK.Joysticks.Length)
        {
            return 0f;
        }

        string joystickName = OuyaSDK.Joysticks[playerIndex];
        if (null == joystickName)
        {
            return 0f;
        }

        bool invert = false;
        string axisName = string.Empty;

        if (joystickName.Equals("Broadcom Bluetooth HID", StringComparison.CurrentCultureIgnoreCase))
        {
            if (SystemInfo.deviceModel.Contains("M.O.J.O."))
            {
                // The M.O.J.O. reports the wrong name for the OUYA Game Controller
                joystickName = "OUYA Game Controller";
            }
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        // on Android always use OUYA Game Controller
        joystickName = "OUYA Game Controller";
#endif

        switch (joystickName.ToUpper())
        {
            case "HARMONIX ROCK BAND DRUMKIT":
                return 0f;
            case "HARMONIX ROCK BAND GUITAR":
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_PICKUP:
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_WHAMMI:
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 7", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_STRUM:
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 8", (int)player);
                        break;
                    default:
                        return 0f;
                }
                break;

            case "BROADCOM BLUETOOTH HID":
            case "MOGA PRO HID":
#if !UNITY_EDITOR && UNITY_ANDROID
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 8", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 7", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    default:
                        return 0f;
                }
#endif
                break;

                       case "MAD CATZ C.T.R.L.R (SMART)":
                               switch (keyCode)
                               {
                               case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                                       axisName = string.Format("Joy{0} Axis 1", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                                       axisName = string.Format("Joy{0} Axis 2", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                                       axisName = string.Format("Joy{0} Axis 3", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                                       axisName = string.Format("Joy{0} Axis 4", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_LT:
                                       axisName = string.Format("Joy{0} Axis 13", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_RT:
                                       axisName = string.Format("Joy{0} Axis 12", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                                       axisName = string.Format("Joy{0} Axis 6", (int)player);
                                       invert = true;
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                                       axisName = string.Format("Joy{0} Axis 5", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                                       axisName = string.Format("Joy{0} Axis 5", (int)player);
                                       break;
                               case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                                       axisName = string.Format("Joy{0} Axis 6", (int)player);
                                       invert = true;
                                       break;
                               default:
                                       return 0f;
                               }
                           break;

            default:
            case "OUYA GAME CONTROLLER":


#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 7", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 8", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        invert = true;
                        break;
                    default:
                        return 0f;
                }
#else
                           switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    default:
                        return 0f;
                }
#endif
#else
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 9", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 10", (int)player);
                        break;
                    default:
                        return 0f;
                }
#endif
                break;

            case "XBOX 360 WIRELESS RECEIVER":

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    default:
                        return 0f;
                }

#else
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                            axisName = string.Format("Joy{0} Axis 1", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 2", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                            axisName = string.Format("Joy{0} Axis 3", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 4", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            axisName = string.Format("Joy{0} Axis 5", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        default:
                            return 0f;
                    }
                }
                else
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                            axisName = string.Format("Joy{0} Axis 1", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 2", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                            axisName = string.Format("Joy{0} Axis 4", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 5", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            axisName = string.Format("Joy{0} Axis 9", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            axisName = string.Format("Joy{0} Axis 10", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            axisName = string.Format("Joy{0} Axis 7", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            axisName = string.Format("Joy{0} Axis 7", (int)player);
                            break;
                        default:
                            return 0f;
                    }
                }
#endif

                break;

            case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
            case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
            case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)":
            case "MICROSOFT X-BOX 360 PAD":
            case "CONTROLLER (XBOX 360 FOR WINDOWS)":
            case "CONTROLLER (XBOX360 GAMEPAD)":
            case "XBOX 360 FOR WINDOWS (CONTROLLER)":
            case "MICROSOFT WIRELESS 360 CONTROLLER":

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 7", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 8", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    default:
                        return 0f;
                }

#else
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                            axisName = string.Format("Joy{0} Axis 1", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 2", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                            axisName = string.Format("Joy{0} Axis 3", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 4", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            axisName = string.Format("Joy{0} Axis 5", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        default:
                            return 0f;
                    }
                }
                else
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                            axisName = string.Format("Joy{0} Axis 1", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 2", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                            axisName = string.Format("Joy{0} Axis 4", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                            axisName = string.Format("Joy{0} Axis 5", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            axisName = string.Format("Joy{0} Axis 9", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            axisName = string.Format("Joy{0} Axis 10", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            axisName = string.Format("Joy{0} Axis 6", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            axisName = string.Format("Joy{0} Axis 7", (int)player);
                            break;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            axisName = string.Format("Joy{0} Axis 7", (int)player);
                            break;
                        default:
                            return 0f;
                    }
                }
#endif

                break;

            case "LOGITECH DUAL ACTION":
            case "LOGITECH LOGITECH DUAL ACTION":

#if !UNITY_EDITOR && UNITY_ANDROID
			
				switch (keyCode)
				{
				case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
					axisName = string.Format("Joy{0} Axis 1", (int)player);
					break;
				case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
					axisName = string.Format("Joy{0} Axis 2", (int)player);
					break;
				case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
					axisName = string.Format("Joy{0} Axis 3", (int)player);
					break;
				case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
					axisName = string.Format("Joy{0} Axis 4", (int)player);
					invert = true;
					break;
				case OuyaSDK.KeyEnum.BUTTON_LT:
					axisName = string.Format("Joy{0} Axis 7", (int)player);
					break;
				case OuyaSDK.KeyEnum.BUTTON_RT:
					axisName = string.Format("Joy{0} Axis 8", (int)player);
					break;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
					axisName = string.Format("Joy{0} Axis 5", (int)player);
					break;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
					axisName = string.Format("Joy{0} Axis 5", (int)player);
					break;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
					axisName = string.Format("Joy{0} Axis 6", (int)player);
					break;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
					axisName = string.Format("Joy{0} Axis 6", (int)player);
					break;
				default:
					return 0f;
				}
				
#else

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        if (GetButton(6, player, Input.GetKey))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        if (GetButton(7, player, Input.GetKey))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    default:
                        return 0f;
                }
#endif

                break;

            case "": //the driver is reporting the controller as blank

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        invert = true;
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 7", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 8", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    default:
                        return 0f;
                }

#else

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
                        axisName = string.Format("Joy{0} Axis 1", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 2", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
                        axisName = string.Format("Joy{0} Axis 3", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
                        axisName = string.Format("Joy{0} Axis 4", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        axisName = string.Format("Joy{0} Axis 5", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        axisName = string.Format("Joy{0} Axis 6", (int)player);
                        break;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        if (GetButton(7, player, Input.GetKey))
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        if (GetButton(8, player, Input.GetKey))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        if (GetButton(5, player, Input.GetKey))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        if (GetButton(6, player, Input.GetKey))
                        {
                            return -1;
                        }
                        else
                        {
                            return 0;
                        }
                    default:
                        return 0f;
                }
#endif

                break;
        }
        if (string.IsNullOrEmpty(axisName))
        {
            return 0f;
        }
        if (invert)
        {
            return -action(axisName);
        }
        else
        {
            return action(axisName);
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public delegate bool GetButtonDelegate(int deviceId, int button);
#else
    public delegate bool GetButtonDelegate(KeyCode keyCode);

    public static string GetKeyCode(int buttonNum, OuyaSDK.OuyaPlayer player)
    {
        switch (player)
        {
            case OuyaSDK.OuyaPlayer.none:
                return string.Format("JoystickButton{0}", buttonNum);
            default:
                return string.Format("Joystick{0}Button{1}", ((int)player), buttonNum);
        }
    }

    public static bool GetButton(int buttonNum, OuyaSDK.OuyaPlayer player)
    {
        return GetButton(buttonNum, player, Input.GetKey);
    }

    public static bool GetButton(int buttonNum, OuyaSDK.OuyaPlayer player, GetButtonDelegate action)
    {
        string keyCode = GetKeyCode(buttonNum, player);
        if (string.IsNullOrEmpty(keyCode))
        {
            return false;
        }
        OuyaKeyCode key = (OuyaKeyCode)Enum.Parse(typeof(OuyaKeyCode), keyCode);
        return action((KeyCode)(int)key);
    }

    public static bool GetButton(OuyaSDK.KeyEnum keyCode)
    {
        return (
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player1) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player2) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player3) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player4) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player5) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player6) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player7) ||
                   GetButton(keyCode, OuyaSDK.OuyaPlayer.player8));
    }

#endif

    public static bool GetButton(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetButtonWithDelegate(keyCode, player, OuyaSDK.OuyaInput.GetButton);
#else
        return GetButtonWithDelegate(keyCode, player, Input.GetKey);
#endif
    }

    public static bool GetButtonUp(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetButtonWithDelegate(keyCode, player, OuyaSDK.OuyaInput.GetButtonUp);
#else
        return GetButtonWithDelegate(keyCode, player, Input.GetKeyUp);
#endif
    }

    public static bool GetButtonDown(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetButtonWithDelegate(keyCode, player, OuyaSDK.OuyaInput.GetButtonDown);
#else
        return GetButtonWithDelegate(keyCode, player, Input.GetKeyDown);
#endif
    }

    private static bool GetButtonWithDelegate(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player, GetButtonDelegate action)
    {
        if (null == OuyaSDK.Joysticks)
        {
            return false;
        }

        int playerIndex = (int)player - 1;

#if UNITY_ANDROID && !UNITY_EDITOR
        switch (keyCode)
        {
            case OuyaSDK.KeyEnum.BUTTON_LB:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_L1);
            case OuyaSDK.KeyEnum.BUTTON_RB:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_R1);
            case OuyaSDK.KeyEnum.BUTTON_O:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_O);
            case OuyaSDK.KeyEnum.BUTTON_U:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_U);
            case OuyaSDK.KeyEnum.BUTTON_Y:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_Y);
            case OuyaSDK.KeyEnum.BUTTON_A:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_A);
            case OuyaSDK.KeyEnum.BUTTON_L3:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_L3);
            case OuyaSDK.KeyEnum.BUTTON_R3:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_R3);
            case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_DPAD_DOWN);
            case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_DPAD_LEFT);
            case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_DPAD_RIGHT);
            case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_DPAD_UP);
            case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                return OuyaSDK.OuyaInput.GetButton(playerIndex, OuyaController.BUTTON_MENU);
            default:
                return false;
        }
#else

        if (playerIndex >= OuyaSDK.Joysticks.Length)
        {
            return false;
        }

        string joystickName = OuyaSDK.Joysticks[playerIndex];
        if (null == joystickName)
        {
            return false;
        }

        if (joystickName.Equals("Broadcom Bluetooth HID", StringComparison.CurrentCultureIgnoreCase))
        {
            if (SystemInfo.deviceModel.Contains("M.O.J.O."))
            {
                // The M.O.J.O. reports the wrong name for the OUYA Game Controller
                joystickName = "OUYA Game Controller";
            }
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        // on Android always use OUYA Game Controller
        joystickName = "OUYA Game Controller";
#endif

        switch (joystickName.ToUpper())
        {
            case "HARMONIX ROCK BAND DRUMKIT":
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_GREEN:
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_A:
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_RED:
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_B:
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_YELLOW:
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_Y:
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_BLUE:
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_X:
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_ORANGE:
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_BACK:
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_START:
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(12, player, action);
                    default:
                        return false;
                }
            case "HARMONIX ROCK BAND GUITAR":
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_GREEN:
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_RED:
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_YELLOW:
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_BLUE:
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_ORANGE:
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_LOWER:
                        return GetButton(13, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_BACK:
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_START:
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(12, player, action);
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_PICKUP:
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return false;
                    case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_WHAMMI:
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return false;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetAxis(keyCode, player) < 0f;
                    default:
                        return false;
                }

            case "BROADCOM BLUETOOTH HID":
            case "MOGA PRO HID":
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(13, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(14, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetAxis(keyCode, player) > 0f;
                    default:
                        return false;
                }

                       case "MAD CATZ C.T.R.L.R (SMART)":
                               switch (keyCode)
                               {
                                       case OuyaSDK.KeyEnum.BUTTON_LB:
                                               return GetButton(4, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_RB:
                                               return GetButton(5, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_O:
                                               return GetButton(0, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_U:
                                               return GetButton(2, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_Y:
                                               return GetButton(3, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_A:
                                               return GetButton(1, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_L3:
                                               return GetButton(8, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_R3:
                                               return GetButton(9, player, action);
                                       case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                                               return GetAxis(keyCode, player) < 0f;
                                       case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                                               return GetAxis(keyCode, player) < 0f;
                                       case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                                               return GetAxis(keyCode, player) > 0f;
                                       case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                                               return GetAxis(keyCode, player) > 0f;
                                       case OuyaSDK.KeyEnum.BUTTON_LT:
                                               return GetAxis(keyCode, player) > 0f;
                                       case OuyaSDK.KeyEnum.BUTTON_RT:
                                               return GetAxis(keyCode, player) > 0f;
                                       default:
                                               return false;
                               }

            default:
            case "OUYA GAME CONTROLLER":

#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1

                               switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(5, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(2, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetButton(9, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetButton(8, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetButton(12, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetButton(13, player, action);
                    default:
                        return false;
                }
#else
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(5, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(2, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetButton(8, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetButton(9, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetButton(12, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetButton(13, player, action);
                    default:
                        return false;
                }
#endif

            case "XBOX 360 WIRELESS RECEIVER":

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(13, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(14, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetButton(2, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetAxis(keyCode, player) > 0f;
                    default:
                        return false;
                }
#else
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.BUTTON_LB:
                            return GetButton(13, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_RB:
                            return GetButton(14, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_O:
                            return GetButton(16, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_U:
                            return GetButton(18, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_Y:
                            return GetButton(19, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_A:
                            return GetButton(17, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_L3:
                            return GetButton(11, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_R3:
                            return GetButton(12, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            return GetButton(5, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            return GetButton(6, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            return GetButton(7, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            return GetButton(8, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            return GetAxis(keyCode, player) > -1f;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            return GetAxis(keyCode, player) > -1f;
                        default:
                            return false;
                    }
                }
                else
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.BUTTON_LB:
                            return GetButton(4, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_RB:
                            return GetButton(5, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_O:
                            return GetButton(0, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_U:
                            return GetButton(2, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_Y:
                            return GetButton(3, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_A:
                            return GetButton(1, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_L3:
                            return GetButton(8, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_R3:
                            return GetButton(9, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            return GetAxis(keyCode, player) < 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            return GetAxis(keyCode, player) < 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            return GetAxis(keyCode, player) > 0f;
                        default:
                            return false;
                    }
                }
#endif

            case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
            case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
            case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)":
            case "MICROSOFT X-BOX 360 PAD":
            case "CONTROLLER (XBOX 360 FOR WINDOWS)":
            case "CONTROLLER (XBOX360 GAMEPAD)":
            case "XBOX 360 FOR WINDOWS (CONTROLLER)":
            case "MICROSOFT WIRELESS 360 CONTROLLER":

#if !UNITY_EDITOR && UNITY_ANDROID

                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(13, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(14, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetAxis(keyCode, player) > 0f;
                    default:
                        return false;
                }
#else
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.BUTTON_LB:
                            return GetButton(13, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_RB:
                            return GetButton(14, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_O:
                            return GetButton(16, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_U:
                            return GetButton(18, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_Y:
                            return GetButton(19, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_A:
                            return GetButton(17, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_L3:
                            return GetButton(11, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_R3:
                            return GetButton(12, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            return GetButton(5, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            return GetButton(6, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            return GetButton(7, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            return GetButton(8, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            return GetAxis(keyCode, player) > -1f;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            return GetAxis(keyCode, player) > -1f;
                        default:
                            return false;
                    }
                }
                else
                {
                    switch (keyCode)
                    {
                        case OuyaSDK.KeyEnum.BUTTON_LB:
                            return GetButton(4, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_RB:
                            return GetButton(5, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_O:
                            return GetButton(0, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_U:
                            return GetButton(2, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_Y:
                            return GetButton(3, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_A:
                            return GetButton(1, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_L3:
                            return GetButton(8, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_R3:
                            return GetButton(9, player, action);
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                            return GetAxis(keyCode, player) < 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                            return GetAxis(keyCode, player) < 0f;
                        case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_LT:
                            return GetAxis(keyCode, player) > 0f;
                        case OuyaSDK.KeyEnum.BUTTON_RT:
                            return GetAxis(keyCode, player) > 0f;
                        default:
                            return false;
                    }
                }
#endif
            case "LOGITECH DUAL ACTION":
            case "LOGITECH LOGITECH DUAL ACTION":

#if !UNITY_EDITOR && UNITY_ANDROID
				
				switch (keyCode)
				{
				case OuyaSDK.KeyEnum.BUTTON_LB:
					return GetButton(4, player, action);
				case OuyaSDK.KeyEnum.BUTTON_RB:
					return GetButton(5, player, action);
				case OuyaSDK.KeyEnum.BUTTON_O:
					return GetButton(1, player, action);
				case OuyaSDK.KeyEnum.BUTTON_U:
					return GetButton(0, player, action);
				case OuyaSDK.KeyEnum.BUTTON_Y:
					return GetButton(3, player, action);
				case OuyaSDK.KeyEnum.BUTTON_A:
					return GetButton(2, player, action);
				case OuyaSDK.KeyEnum.BUTTON_L3:
					return GetButton(10, player, action);
				case OuyaSDK.KeyEnum.BUTTON_R3:
					return GetButton(11, player, action);
				case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
					return GetAxis(keyCode, player) > 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
					return GetAxis(keyCode, player) < 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
					return GetAxis(keyCode, player) < 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
					return GetAxis(keyCode, player) > 0f;
				case OuyaSDK.KeyEnum.BUTTON_LT:
					return GetAxis(keyCode, player) > 0f;
				case OuyaSDK.KeyEnum.BUTTON_RT:
					return GetAxis(keyCode, player) > 0f;
				default:
					return false;
				}
#else
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(4, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(5, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(1, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(0, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(3, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(2, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetAxis(keyCode, player) < 0f;
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_SELECT:
                        return GetButton(8, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_START:
                        return GetButton(9, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                        return GetButton(15, player, action);
                    default:
                        return false;
                }
#endif
            case "": //the driver is reporting the controller as blank

#if !UNITY_EDITOR && UNITY_ANDROID
				
				switch (keyCode)
				{
				case OuyaSDK.KeyEnum.BUTTON_LB:
					return GetButton(6, player, action);
				case OuyaSDK.KeyEnum.BUTTON_RB:
					return GetButton(7, player, action);
				case OuyaSDK.KeyEnum.BUTTON_O:
					return GetButton(0, player, action);
				case OuyaSDK.KeyEnum.BUTTON_U:
					return GetButton(3, player, action);
				case OuyaSDK.KeyEnum.BUTTON_Y:
					return GetButton(4, player, action);
				case OuyaSDK.KeyEnum.BUTTON_A:
					return GetButton(1, player, action);
				case OuyaSDK.KeyEnum.BUTTON_L3:
					return GetButton(13, player, action);
				case OuyaSDK.KeyEnum.BUTTON_R3:
					return GetButton(14, player, action);
				case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
					return GetAxis(keyCode, player) > 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
					return GetAxis(keyCode, player) < 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
					return GetAxis(keyCode, player) < 0f;
				case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
					return GetAxis(keyCode, player) > 0f;
				case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetAxis(keyCode, player) > 0f;
                    default:
                        return false;
                }
#else
                switch (keyCode)
                {
                    case OuyaSDK.KeyEnum.BUTTON_LB:
                        return GetButton(13, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_RB:
                        return GetButton(14, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_O:
                        return GetButton(16, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_U:
                        return GetButton(18, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        return GetButton(19, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_A:
                        return GetButton(17, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_L3:
                        return GetButton(11, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_R3:
                        return GetButton(12, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        return GetButton(5, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        return GetButton(6, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        return GetButton(7, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        return GetButton(8, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_LT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_RT:
                        return GetAxis(keyCode, player) > 0f;
                    case OuyaSDK.KeyEnum.BUTTON_SELECT:
                        return GetButton(10, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_START:
                        return GetButton(9, player, action);
                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                        return GetButton(15, player, action);
                    default:
                        return false;
                }
#endif
        }

        return false;
#endif
    }

    #endregion
}
