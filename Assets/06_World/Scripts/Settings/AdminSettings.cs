/// <summary>
/// [world]
/// 환경설정 _ 관리자 설정 Script
/// @author         : HJ Lee
/// @last update    : 2024. 01. 02
/// @version        : 0.1
/// @update
///     v0.1 (2024. 01. 02) : 최초 생성
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    public class AdminSettings : WindowPage
    {
        private const string TAG = "AdminSettings";

        [Header("module")]
        [SerializeField] private SettingModule _module;
    }
}