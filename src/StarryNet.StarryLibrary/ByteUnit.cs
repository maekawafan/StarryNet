namespace StarryNet.StarryLibrary
{
    public struct ByteUnit
    {
        private const ulong kiloByteUnit = 1024ul;
        private const ulong megaByteUnit = 1048576ul;
        private const ulong gigaByteUnit = 1073741824ul;
        private const ulong teraByteUnit = 1099511627776ul;
        private const ulong petaByteUnit = 1125899906842624ul;

        public ulong size;

        public ulong KB
        {
            get { return size / kiloByteUnit; }
            set { size = value * kiloByteUnit; }
        }

        public ulong MB
        {
            get { return size / megaByteUnit; }
            set { size = value * megaByteUnit; }
        }

        public ulong GB
        {
            get { return size / gigaByteUnit; }
            set { size = value * gigaByteUnit; }
        }

        public ulong TB
        {
            get { return size / teraByteUnit; }
            set { size = value * teraByteUnit; }
        }

        public ulong PB
        {
            get { return size / petaByteUnit; }
            set { size = value * petaByteUnit; }
        }

        public ByteUnit(ulong size)
        {
            this.size = size;
        }

        public static ByteUnit Zero()
        {
            return new ByteUnit(0);
        }

        public static ByteUnit KiloByte(ulong kb)
        {
            return new ByteUnit() { KB = kb };
        }

        public static ByteUnit MegaByte(ulong mb)
        {
            return new ByteUnit() { MB = mb };
        }

        public static ByteUnit GigaByte(ulong gb)
        {
            return new ByteUnit() { GB = gb };
        }

        public static ByteUnit TeraByte(ulong tb)
        {
            return new ByteUnit() { TB = tb };
        }

        public static ByteUnit PetaByte(ulong pb)
        {
            return new ByteUnit() { PB = pb };
        }

        public override string ToString()
        {
            if (size < 1024UL)
                return $"{size}Byte";
            if (size < kiloByteUnit)
                return $"{KB}KB";
            if (size < megaByteUnit)
                return $"{MB}MB";
            if (size < gigaByteUnit)
                return $"{GB}GB";
            if (size < teraByteUnit)
                return $"{TB}TB";
            if (size < petaByteUnit)
                return $"{PB}PB";
            return $"{size}Byte";
        }

        public string ToString(string format)
        {
            switch (format.ToUpper())
            {
                case "KB":
                    return $"{KB}KB";
                case "MB":
                    return $"{MB}MB";
                case "GB":
                    return $"{GB}GB";
                case "TB":
                    return $"{TB}TB";
                case "PB":
                    return $"{PB}PB";
            }
            return $"{size}byte";
        }
    }
}