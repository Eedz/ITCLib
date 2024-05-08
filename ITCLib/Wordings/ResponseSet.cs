﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class ResponseSet : ObservableObject
    {
        public string RespSetName
        {
            get => _respsetname;
            set => SetProperty(ref _respsetname, value);
        }

        public ResponseType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string RespList
        {
            get => _respList;
            set
            {
                SetProperty(ref _respList, value.Replace("&nbsp;", " "));
                RespListR = _respList;
                RespListR = Utilities.FormatText(RespListR);
            }
        }

        public string FieldType
        {
            get
            {
                switch (Type)
                {
                    case ResponseType.RespOptions:
                        return "RespOptions";
                    case ResponseType.NRCodes:
                        return "NRCodes";
                    default:
                        return null;
                }
            }
        }

        public string RespListR { get; private set; }

        public List<ResponseOption> Options { get; private set; }


        public ResponseSet()
        {
            RespSetName = "0";
            RespList = string.Empty;
            Options = new List<ResponseOption>();
        }

        public ResponseSet(ResponseType type)
        {
            RespSetName = "0";
            RespList = string.Empty;
            Options = new List<ResponseOption>();
            Type = type;
        }


        public ResponseSet(string setname, ResponseType type, string responseText)
        {
            RespSetName = setname;
            Type = type;
            RespList = responseText;
            Options = new List<ResponseOption>();
        }

        public void SetRandomName()
        {
            this.RespSetName = GenerateRandomString(5);
        }

        string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        public override string ToString()
        {
            return this.FieldType + "# " + this.RespSetName;
        }

        public override bool Equals(object obj)
        {
            var respset = obj as ResponseSet;
            return respset != null &&
                   RespSetName.ToLower() == respset.RespSetName.ToLower() &&
                   Type == respset.Type &&
                   RespList == respset.RespList;
        }

        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + RespSetName.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RespList);
            return hashCode;
        }

        private string _respsetname;
        private ResponseType _type;
        private string _respList;
    }

    public class ResponseOption
    {
        public string Code { get; set; }
        public string Label { get; set; }

        public ResponseOption() 
        {
            Code = string.Empty;
            Label = string.Empty;
        }

        public ResponseOption(string code, string label)
        {
            Code = code;
            Label = label;
        }

    }
}
