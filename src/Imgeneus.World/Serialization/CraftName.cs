using BinarySerialization;

namespace Imgeneus.World.Serialization
{
    public struct CraftName
    {
        /// <summary>
        /// Previously stored in database as byte.
        /// ['0','1'] => "01" means item has 1 additional strength.
        /// ['2','1'] => "21" means item has 21 additional strength.
        /// </summary>
        [FieldOrder(0)]
        public byte[] EnchantedSTR;

        /// <summary>
        /// Check EnchantedSTR for more info.
        /// </summary>
        [FieldOrder(1)]
        public byte[] EnchantedDEX;

        /// <summary>
        /// Check EnchantedSTR for more info.
        /// </summary>
        [FieldOrder(2)]
        public byte[] EnchantedREC;

        /// <summary>
        /// Check EnchantedSTR for more info.
        /// </summary>
        [FieldOrder(3)]
        public byte[] EnchantedINT;

        /// <summary>
        /// Check EnchantedSTR for more info.
        /// </summary>
        [FieldOrder(4)]
        public byte[] EnchantedWIS;

        /// <summary>
        /// Check EnchantedSTR for more info.
        /// </summary>
        [FieldOrder(5)]
        public byte[] EnchantedLUC;

        /// <summary>
        /// Previously stored in database as byte.
        /// ['0','7'] => "07" means item has 700 additional health points.
        /// ['7','0'] => "70" means item has 7000 additional health points.
        /// </summary>
        [FieldOrder(6)]
        public byte[] EnchantedHP;

        /// <summary>
        /// Check EnchantedHP for more info.
        /// </summary>
        [FieldOrder(7)]
        public byte[] EnchantedMP;

        /// <summary>
        /// Check EnchantedHP for more info.
        /// </summary>
        [FieldOrder(8)]
        public byte[] EnchantedSP;

        /// <summary>
        /// 1-20 step.
        /// ['0','1'] => 1 step
        /// ['0','2'] => 2 step
        /// ['2','0'] => 20 step
        /// </summary>
        [FieldOrder(9)]
        public byte[] EnchantedStep;

        public CraftName(
            char str1, char str2,
            char dex1, char dex2,
            char rec1, char rec2,
            char int1, char int2,
            char wis1, char wis2,
            char luc1, char luc2,
            char hp1, char hp2,
            char mp1, char mp2,
            char sp1, char sp2,
            char step1, char step2)
        {
            EnchantedSTR = new byte[2] { (byte)str1, (byte)str2 };
            EnchantedDEX = new byte[2] { (byte)dex1, (byte)dex2 };
            EnchantedREC = new byte[2] { (byte)rec1, (byte)rec2 };
            EnchantedINT = new byte[2] { (byte)int1, (byte)int2 };
            EnchantedWIS = new byte[2] { (byte)wis1, (byte)wis2 };
            EnchantedLUC = new byte[2] { (byte)luc1, (byte)luc2 };
            EnchantedHP = new byte[2] { (byte)hp1, (byte)hp2 };
            EnchantedMP = new byte[2] { (byte)mp1, (byte)mp2 };
            EnchantedSP = new byte[2] { (byte)sp1, (byte)sp2 };
            EnchantedStep = new byte[2] { (byte)step1, (byte)step2 };
        }
    }
}
