using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SharpDX.XInput;


namespace Kodari
{
    enum BeepSounds
    {
        Run = 2000,
        Exit = 250,
        Connect = 1000,
        UnConnect = 500,
    }
    static class BeepSound
    {
        public static void Play(BeepSounds mode)
        {
            Console.Beep((int)mode, 150);
        }
    }
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void Keybd_event(byte vk, byte scan, int flags, ref int extrainfo);
        const byte TriggerValue = 255 / 2;
        static bool isRun = true;
        static bool toggled = false;
        static bool connected = false;
        static int exitConut = 0;
        static Controller controller;
        static void Main(string[] args)
        {
            BeepSound.Play(BeepSounds.Run);
            // 컨트롤러 연결
            controller = new Controller(UserIndex.One);
            // 연결 확인
            if (controller.IsConnected)
            {
                BeepSound.Play(BeepSounds.Connect);
                connected = true;
            }
            else
            {
                goto EXIT;
            }
            // 종료키 등록

            // 루프
            while (isRun)
            {
                // 연결이 끊어졌다가 연결 됨.
                if (!connected&&controller.IsConnected)
                {
                    BeepSound.Play(BeepSounds.Connect);
                    connected = controller.IsConnected;
                    exitConut = 0;
                }
                    //연결이 끊어짐
                else if (connected && !controller.IsConnected)
                {
                    BeepSound.Play(BeepSounds.UnConnect);
                    connected = controller.IsConnected;
                }
                // 패드가 연결중임
                if (connected)
                {
                    // RT가 50%이상 입력시
                    if (!toggled && controller.GetState().Gamepad.RightTrigger >= TriggerValue)
                    {
                        KorToggle();
                        toggled = true;
                    }
                    // RT가 50%미만 입력시
                    if (toggled && controller.GetState().Gamepad.RightTrigger < TriggerValue)
                    {
                        KorToggle();
                        toggled = false;
                    }
                    if (controller.GetState().Gamepad.LeftTrigger >= TriggerValue)
                    {
                        break;
                    }
                }
                else
                {
                    // 연결이 끊어졌으면 종료카운트가 증가
                    exitConut++;
                    // 종료 카운트가 100이면 종료
                    if (exitConut >= 100)
                    {
                        break;
                    }
                }
                // 대기
                Thread.Sleep(100);
            }

        EXIT:
            Exit();
        }
        static void Exit()
        {
            if (controller.IsConnected)
            {
                BeepSound.Play(BeepSounds.UnConnect);
                controller = null;
            }
            BeepSound.Play(BeepSounds.Exit);
        }
        static void KorToggle()
        {
            int info = 0;
            Keybd_event(0x15, 0, 0, ref info);
        }
    }
}
