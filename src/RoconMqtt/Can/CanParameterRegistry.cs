using RoconMqtt.Can.Models;

namespace RoconMqtt.Can;

public static class CanParameterRegistry
{
    #region Parameters

    private static readonly Dictionary<InfoNumber, ParameterDefinition> _parameters =
        new()
        {
            {
                new InfoNumber(0x01, 0x48),
                new ParameterDefinition(
                    Name: "cGERAETE_KENNUNG",
                    InfoNumber: new InfoNumber(0x01, 0x48),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x0C),
                new ParameterDefinition(
                    Name: "cAUSSENTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x0C),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x0A, 0x0C),
                new ParameterDefinition(
                    Name: "cAUSSENTEMP_WAERMEPUMPE",
                    InfoNumber: new InfoNumber(0x0A, 0x0C),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x22),
                new ParameterDefinition(
                    Name: "cTAG",
                    InfoNumber: new InfoNumber(0x01, 0x22),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x23),
                new ParameterDefinition(
                    Name: "cMONAT",
                    InfoNumber: new InfoNumber(0x01, 0x23),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x24),
                new ParameterDefinition(
                    Name: "cJAHR",
                    InfoNumber: new InfoNumber(0x01, 0x24),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x25),
                new ParameterDefinition(
                    Name: "cSTUNDE",
                    InfoNumber: new InfoNumber(0x01, 0x25),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x26),
                new ParameterDefinition(
                    Name: "cMINUTE",
                    InfoNumber: new InfoNumber(0x01, 0x26),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x12),
                new ParameterDefinition(
                    Name: "cPROGRAMMSCHALTER",
                    InfoNumber: new InfoNumber(0x01, 0x12),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: new Dictionary<int, string>
                    {
                        { 1, "1" },
                        { 3, "3" },
                        { 4, "4" },
                        { 5, "5" },
                        { 11, "11" },
                        { 12, "12" },
                        { 17, "17" },
                    },
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x0E),
                new ParameterDefinition(
                    Name: "cSPEICHERISTTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x0E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: true,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x03),
                new ParameterDefinition(
                    Name: "cVERSTELLTE_SPEICHERSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x03),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 10,
                    Max: 70,
                    Default: 48,
                    Display: true,
                    WaterCircuit: true,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x13),
                new ParameterDefinition(
                    Name: "cEINSTELL_SPEICHERSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x13),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 10,
                    Max: 70,
                    Default: 48,
                    Display: true,
                    WaterCircuit: true,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x0A, 0x06),
                new ParameterDefinition(
                    Name: "cEINSTELL_SPEICHERSOLLTEMP2",
                    InfoNumber: new InfoNumber(0x0A, 0x06),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 10,
                    Max: 70,
                    Default: 48,
                    Display: true,
                    WaterCircuit: true,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x3E),
                new ParameterDefinition(
                    Name: "cEINSTELL_SPEICHERSOLLTEMP3",
                    InfoNumber: new InfoNumber(0x01, 0x3E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 10,
                    Max: 70,
                    Default: 48,
                    Display: true,
                    WaterCircuit: true,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x00),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1",
                    InfoNumber: new InfoNumber(0x17, 0x00),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x10),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO",
                    InfoNumber: new InfoNumber(0x17, 0x10),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x11),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x11),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x12),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x12),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x20),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DI",
                    InfoNumber: new InfoNumber(0x17, 0x20),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x21),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x21),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x22),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x22),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x30),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MI",
                    InfoNumber: new InfoNumber(0x17, 0x30),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x31),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x31),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x32),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x32),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x40),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DO",
                    InfoNumber: new InfoNumber(0x17, 0x40),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x41),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x41),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x42),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x42),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x50),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_FR",
                    InfoNumber: new InfoNumber(0x17, 0x50),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x51),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x51),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x52),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x52),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x60),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA",
                    InfoNumber: new InfoNumber(0x17, 0x60),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x61),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x61),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x62),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x62),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x70),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SO",
                    InfoNumber: new InfoNumber(0x17, 0x70),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-23:45"
                )
            },
            {
                new InfoNumber(0x17, 0x71),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x71),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x72),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x72),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x17, 0x80),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_FR",
                    InfoNumber: new InfoNumber(0x17, 0x80),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x81),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x81),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x82),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x82),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x90),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA_SO",
                    InfoNumber: new InfoNumber(0x17, 0x90),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x91),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0x91),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0x92),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_SA_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0x92),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xA0),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_SO",
                    InfoNumber: new InfoNumber(0x17, 0xA0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xA1),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0xA1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xA2),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0xA2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xB0),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_DO",
                    InfoNumber: new InfoNumber(0x17, 0xB0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xB1),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x17, 0xB1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x17, 0xB2),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_1_MO_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x17, 0xB2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x00),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2",
                    InfoNumber: new InfoNumber(0x18, 0x00),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x10),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO",
                    InfoNumber: new InfoNumber(0x18, 0x10),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "05:00-21:00"
                )
            },
            {
                new InfoNumber(0x18, 0x11),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x11),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x12),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x12),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x20),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DI",
                    InfoNumber: new InfoNumber(0x18, 0x20),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "05:00-21:00"
                )
            },
            {
                new InfoNumber(0x18, 0x21),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x21),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x22),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x22),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x30),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MI",
                    InfoNumber: new InfoNumber(0x18, 0x30),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "05:00-21:00"
                )
            },
            {
                new InfoNumber(0x18, 0x31),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x31),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x32),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x32),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x40),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DO",
                    InfoNumber: new InfoNumber(0x18, 0x40),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "05:00-21:00"
                )
            },
            {
                new InfoNumber(0x18, 0x41),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x41),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x42),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x42),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x50),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_FR",
                    InfoNumber: new InfoNumber(0x18, 0x50),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x51),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x51),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x52),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x52),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x60),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA",
                    InfoNumber: new InfoNumber(0x18, 0x60),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-22:00"
                )
            },
            {
                new InfoNumber(0x18, 0x61),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x61),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x62),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x62),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x70),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SO",
                    InfoNumber: new InfoNumber(0x18, 0x70),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-22:00"
                )
            },
            {
                new InfoNumber(0x18, 0x71),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x71),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x72),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x72),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x18, 0x80),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_FR",
                    InfoNumber: new InfoNumber(0x18, 0x80),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x81),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x81),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x82),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x82),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x90),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA_SO",
                    InfoNumber: new InfoNumber(0x18, 0x90),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x91),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0x91),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0x92),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_SA_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0x92),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xA0),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_SO",
                    InfoNumber: new InfoNumber(0x18, 0xA0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xA1),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0xA1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xA2),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0xA2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xB0),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_DO",
                    InfoNumber: new InfoNumber(0x18, 0xB0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xB1),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x18, 0xB1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x18, 0xB2),
                new ParameterDefinition(
                    Name: "cW_WASSERPROG_2_MO_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x18, 0xB2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: true,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x44),
                new ParameterDefinition(
                    Name: "cEINMAL_WW_AKTIV",
                    InfoNumber: new InfoNumber(0x01, 0x44),
                    Type: ParameterType.Bool,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: 0,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x5E),
                new ParameterDefinition(
                    Name: "cWW_AKTIV",
                    InfoNumber: new InfoNumber(0x00, 0x5E),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x11),
                new ParameterDefinition(
                    Name: "cRAUMISTTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x11),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x12),
                new ParameterDefinition(
                    Name: "cVERSTELLTE_RAUMSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x12),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x05),
                new ParameterDefinition(
                    Name: "cRAUMSOLLTEMP_I",
                    InfoNumber: new InfoNumber(0x00, 0x05),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: 20,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x06),
                new ParameterDefinition(
                    Name: "cRAUMSOLLTEMP_II",
                    InfoNumber: new InfoNumber(0x00, 0x06),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: 20,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x07),
                new ParameterDefinition(
                    Name: "cRAUMSOLLTEMP_III",
                    InfoNumber: new InfoNumber(0x00, 0x07),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: 20,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x08),
                new ParameterDefinition(
                    Name: "eNACHTRAUMSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x08),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: 15,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x00),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1",
                    InfoNumber: new InfoNumber(0x14, 0x00),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x10),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO",
                    InfoNumber: new InfoNumber(0x14, 0x10),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-22:00"
                )
            },
            {
                new InfoNumber(0x14, 0x11),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x11),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x12),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x12),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x20),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DI",
                    InfoNumber: new InfoNumber(0x14, 0x20),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-22:00"
                )
            },
            {
                new InfoNumber(0x14, 0x21),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x21),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x22),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x22),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x30),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MI",
                    InfoNumber: new InfoNumber(0x14, 0x30),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-22:00"
                )
            },
            {
                new InfoNumber(0x14, 0x31),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x31),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x32),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x32),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x40),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DO",
                    InfoNumber: new InfoNumber(0x14, 0x40),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x41),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x41),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x42),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x42),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x50),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_FR",
                    InfoNumber: new InfoNumber(0x14, 0x50),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x51),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x51),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x52),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x52),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x60),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA",
                    InfoNumber: new InfoNumber(0x14, 0x60),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "07:00-23:00"
                )
            },
            {
                new InfoNumber(0x14, 0x61),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x61),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x62),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x62),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x70),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SO",
                    InfoNumber: new InfoNumber(0x14, 0x70),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "07:00-23:00"
                )
            },
            {
                new InfoNumber(0x14, 0x71),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x71),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x72),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x72),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x14, 0x80),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_FR",
                    InfoNumber: new InfoNumber(0x14, 0x80),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x81),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x81),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x82),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x82),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x90),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA_SO",
                    InfoNumber: new InfoNumber(0x14, 0x90),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x91),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0x91),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0x92),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_SA_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0x92),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xA0),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_SO",
                    InfoNumber: new InfoNumber(0x14, 0xA0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xA1),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0xA1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xA2),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0xA2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xB0),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_DO",
                    InfoNumber: new InfoNumber(0x14, 0xB0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xB1),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x14, 0xB1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x14, 0xB2),
                new ParameterDefinition(
                    Name: "cHEIZPROG_1_MO_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x14, 0xB2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x00),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2",
                    InfoNumber: new InfoNumber(0x15, 0x00),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x10),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO",
                    InfoNumber: new InfoNumber(0x15, 0x10),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-08:00"
                )
            },
            {
                new InfoNumber(0x15, 0x11),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x11),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x12),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x12),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x20),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DI",
                    InfoNumber: new InfoNumber(0x15, 0x20),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-08:00"
                )
            },
            {
                new InfoNumber(0x15, 0x21),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x21),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x22),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x22),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x30),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MI",
                    InfoNumber: new InfoNumber(0x15, 0x30),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-08:00"
                )
            },
            {
                new InfoNumber(0x15, 0x31),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MI_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x31),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x32),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MI_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x32),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x40),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DO",
                    InfoNumber: new InfoNumber(0x15, 0x40),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "06:00-08:00"
                )
            },
            {
                new InfoNumber(0x15, 0x41),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x41),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x42),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x42),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x50),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_FR",
                    InfoNumber: new InfoNumber(0x15, 0x50),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x51),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x51),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x52),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x52),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x60),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA",
                    InfoNumber: new InfoNumber(0x15, 0x60),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "07:00-23:00"
                )
            },
            {
                new InfoNumber(0x15, 0x61),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x61),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x62),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x62),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "00:00-00:00"
                )
            },
            {
                new InfoNumber(0x15, 0x70),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SO",
                    InfoNumber: new InfoNumber(0x15, 0x70),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: "07:00-23:00"
                )
            },
            {
                new InfoNumber(0x15, 0x71),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x71),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x72),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x72),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x80),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_FR",
                    InfoNumber: new InfoNumber(0x15, 0x80),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x81),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_FR_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x81),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x82),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_FR_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x82),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x90),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA_SO",
                    InfoNumber: new InfoNumber(0x15, 0x90),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x91),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0x91),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0x92),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_SA_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0x92),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xA0),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_SO",
                    InfoNumber: new InfoNumber(0x15, 0xA0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xA1),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_SO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0xA1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xA2),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_SO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0xA2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xB0),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_DO",
                    InfoNumber: new InfoNumber(0x15, 0xB0),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xB1),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_DO_SCHALT_2",
                    InfoNumber: new InfoNumber(0x15, 0xB1),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x15, 0xB2),
                new ParameterDefinition(
                    Name: "cHEIZPROG_2_MO_DO_SCHALT_3",
                    InfoNumber: new InfoNumber(0x15, 0xB2),
                    Type: ParameterType.TimeRange,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x99),
                new ParameterDefinition(
                    Name: "cSOFTWARE_NUMMER",
                    InfoNumber: new InfoNumber(0x01, 0x99),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x9A),
                new ParameterDefinition(
                    Name: "cSOFTWARE_VERSION",
                    InfoNumber: new InfoNumber(0x01, 0x9A),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x02, 0x4B),
                new ParameterDefinition(
                    Name: "cSOFTWARE_UNTERINDEX",
                    InfoNumber: new InfoNumber(0x02, 0x4B),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x58),
                new ParameterDefinition(
                    Name: "cMODUS_PARTY_DAUER",
                    InfoNumber: new InfoNumber(0x13, 0x58),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 360,
                    Default: 0,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x1B),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ANFANG_TAG",
                    InfoNumber: new InfoNumber(0x01, 0x1B),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 31,
                    Default: 2,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x1C),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ANFANG_MONAT",
                    InfoNumber: new InfoNumber(0x01, 0x1C),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 12,
                    Default: 1,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x1D),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ANFANG_JAHR",
                    InfoNumber: new InfoNumber(0x01, 0x1D),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 49,
                    Default: 1,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x1E),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ENDE_TAG",
                    InfoNumber: new InfoNumber(0x01, 0x1E),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 31,
                    Default: 3,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x1F),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ENDE_MONAT",
                    InfoNumber: new InfoNumber(0x01, 0x1F),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 12,
                    Default: 1,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x20),
                new ParameterDefinition(
                    Name: "cMODUS_URLAUB_ENDE_JAHR",
                    InfoNumber: new InfoNumber(0x01, 0x20),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: 0,
                    Max: 49,
                    Default: 1,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x0D),
                new ParameterDefinition(
                    Name: "cKESSELISTTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x0D),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x02),
                new ParameterDefinition(
                    Name: "cKESSELSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x02),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 40,
                    Max: 80,
                    Default: 55,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x16),
                new ParameterDefinition(
                    Name: "cRUECKLAUFTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x16),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0xDA),
                new ParameterDefinition(
                    Name: "cVOLUMENSTROM",
                    InfoNumber: new InfoNumber(0x01, 0xDA),
                    Type: ParameterType.Float,
                    Factor: 1,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xF7),
                new ParameterDefinition(
                    Name: "cPWM_SIGNAL",
                    InfoNumber: new InfoNumber(0xC0, 0xF7),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x9B),
                new ParameterDefinition(
                    Name: "cMISCHERSTELLUNG_1_3UV1",
                    InfoNumber: new InfoNumber(0x06, 0x9B),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xFB),
                new ParameterDefinition(
                    Name: "cMISCHERSTELLUNG_2_3UVB",
                    InfoNumber: new InfoNumber(0xC0, 0xFB),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC1, 0x02),
                new ParameterDefinition(
                    Name: "cT_TVBH1",
                    InfoNumber: new InfoNumber(0xC1, 0x02),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xFE),
                new ParameterDefinition(
                    Name: "cT_TVBHMIX",
                    InfoNumber: new InfoNumber(0xC0, 0xFE),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC1, 0xBF),
                new ParameterDefinition(
                    Name: "cT_TVBH",
                    InfoNumber: new InfoNumber(0xC1, 0xBF),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xF6),
                new ParameterDefinition(
                    Name: "cDEFROST_AKTIV",
                    InfoNumber: new InfoNumber(0xC0, 0xF6),
                    Type: ParameterType.Enum,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0xA4),
                new ParameterDefinition(
                    Name: "cPUMPENLAUFZEIT",
                    InfoNumber: new InfoNumber(0x06, 0xA4),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0xA5),
                new ParameterDefinition(
                    Name: "cKOMPRESSORLAUFZEIT",
                    InfoNumber: new InfoNumber(0x06, 0xA5),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x61),
                new ParameterDefinition(
                    Name: "cVMIN_A1",
                    InfoNumber: new InfoNumber(0x06, 0x61),
                    Type: ParameterType.Float,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x88),
                new ParameterDefinition(
                    Name: "cVMIN_WP",
                    InfoNumber: new InfoNumber(0x06, 0x88),
                    Type: ParameterType.Float,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0x9D),
                new ParameterDefinition(
                    Name: "cVMIN_GCU",
                    InfoNumber: new InfoNumber(0xC0, 0x9D),
                    Type: ParameterType.Float,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x88),
                new ParameterDefinition(
                    Name: "cFEHLER_AKTUELL",
                    InfoNumber: new InfoNumber(0x13, 0x88),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x0A, 0x1F),
                new ParameterDefinition(
                    Name: "eZEITMASTER",
                    InfoNumber: new InfoNumber(0x0A, 0x1F),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xB3),
                new ParameterDefinition(
                    Name: "eSCHALTSCHWELLE_TDHW",
                    InfoNumber: new InfoNumber(0xC0, 0xB3),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xB1),
                new ParameterDefinition(
                    Name: "eSONDERFKT_SCHALTKONTAKT",
                    InfoNumber: new InfoNumber(0xC0, 0xB1),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xC0, 0xB2),
                new ParameterDefinition(
                    Name: "eWARTEZEIT_SONDERFKT",
                    InfoNumber: new InfoNumber(0xC0, 0xB2),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x07, 0x26),
                new ParameterDefinition(
                    Name: "eWASSER_MAX_DRUCKVERLUST",
                    InfoNumber: new InfoNumber(0x07, 0x26),
                    Type: ParameterType.Float,
                    Factor: 1000,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x07, 0x27),
                new ParameterDefinition(
                    Name: "eWASSER_MAXIMALDRUCK",
                    InfoNumber: new InfoNumber(0x07, 0x27),
                    Type: ParameterType.Float,
                    Factor: 1000,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x07, 0x28),
                new ParameterDefinition(
                    Name: "eWASSER_MINIMALDRUCK",
                    InfoNumber: new InfoNumber(0x07, 0x28),
                    Type: ParameterType.Float,
                    Factor: 1000,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x07, 0x25),
                new ParameterDefinition(
                    Name: "eWASSER_SOLLDRUCK",
                    InfoNumber: new InfoNumber(0x07, 0x25),
                    Type: ParameterType.Float,
                    Factor: 1000,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x96),
                new ParameterDefinition(
                    Name: "eWP_FLUESTERBETRIEB",
                    InfoNumber: new InfoNumber(0x06, 0x96),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x69),
                new ParameterDefinition(
                    Name: "eWP_LEISTUNG_HEIZSTAB_S1",
                    InfoNumber: new InfoNumber(0x06, 0x69),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x6A),
                new ParameterDefinition(
                    Name: "eWP_LEISTUNG_HEIZSTAB_S2",
                    InfoNumber: new InfoNumber(0x06, 0x6A),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x6B),
                new ParameterDefinition(
                    Name: "eWP_LEISTUNG_HZU_BIV",
                    InfoNumber: new InfoNumber(0x06, 0x6B),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x6E),
                new ParameterDefinition(
                    Name: "eWP_MAX_TEMP_HEIZUNG",
                    InfoNumber: new InfoNumber(0x06, 0x6E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x82),
                new ParameterDefinition(
                    Name: "eWP_MOD_HYST_DURCHFLUSS",
                    InfoNumber: new InfoNumber(0x06, 0x82),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0xA0),
                new ParameterDefinition(
                    Name: "eWP_SOLLWERT_ANPASSUNG_HEIZEN",
                    InfoNumber: new InfoNumber(0x06, 0xA0),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0xA1),
                new ParameterDefinition(
                    Name: "eWP_SOLLWERT_ANPASSUNG_KUEHLEN",
                    InfoNumber: new InfoNumber(0x06, 0xA1),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x83),
                new ParameterDefinition(
                    Name: "eWP_SPREIZUNG_HZ_BETRIEB",
                    InfoNumber: new InfoNumber(0x06, 0x83),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x84),
                new ParameterDefinition(
                    Name: "eWP_SPREIZUNG_WW_BETRIEB",
                    InfoNumber: new InfoNumber(0x06, 0x84),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x8C),
                new ParameterDefinition(
                    Name: "eWP_START_MAX_TEMP",
                    InfoNumber: new InfoNumber(0x06, 0x8C),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x85),
                new ParameterDefinition(
                    Name: "eWP_VERZ_ZEIT_PUMPE",
                    InfoNumber: new InfoNumber(0x06, 0x85),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x2E),
                new ParameterDefinition(
                    Name: "eABSENKOPTIMIERUNG",
                    InfoNumber: new InfoNumber(0x01, 0x2E),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x15),
                new ParameterDefinition(
                    Name: "eADAPTION",
                    InfoNumber: new InfoNumber(0x01, 0x15),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x03),
                new ParameterDefinition(
                    Name: "eAUFHEIZOPTIMIERUNG",
                    InfoNumber: new InfoNumber(0x01, 0x03),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x0A, 0x00),
                new ParameterDefinition(
                    Name: "eFROSTSCHUTZTEMP",
                    InfoNumber: new InfoNumber(0x0A, 0x00),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x17),
                new ParameterDefinition(
                    Name: "eHEIZGRENZE_NACHT",
                    InfoNumber: new InfoNumber(0x01, 0x17),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x16),
                new ParameterDefinition(
                    Name: "eHEIZGRENZE_TAG",
                    InfoNumber: new InfoNumber(0x01, 0x16),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x41),
                new ParameterDefinition(
                    Name: "eHZK_FUNKTION",
                    InfoNumber: new InfoNumber(0x01, 0x41),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x0E),
                new ParameterDefinition(
                    Name: "eHZKKURVE",
                    InfoNumber: new InfoNumber(0x01, 0x0E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x59),
                new ParameterDefinition(
                    Name: "eKUEHLSOLLWERT_KORR_HZK_0",
                    InfoNumber: new InfoNumber(0x13, 0x59),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x10),
                new ParameterDefinition(
                    Name: "eMAX_AUFHEIZVORVERLEGUNG",
                    InfoNumber: new InfoNumber(0x01, 0x10),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x5C),
                new ParameterDefinition(
                    Name: "eMAX_KUEHLEN_AUSSENTEMP_HZK0",
                    InfoNumber: new InfoNumber(0x13, 0x5C),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x28),
                new ParameterDefinition(
                    Name: "eMAX_VORLAUFTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x28),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x2B),
                new ParameterDefinition(
                    Name: "eMIN_VORLAUFTEMP",
                    InfoNumber: new InfoNumber(0x01, 0x2B),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x0F),
                new ParameterDefinition(
                    Name: "eRAUMEINFLUSS",
                    InfoNumber: new InfoNumber(0x01, 0x0F),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0xB5),
                new ParameterDefinition(
                    Name: "eSTART_KUEHLEN_AUSSENTEMP_HZK0",
                    InfoNumber: new InfoNumber(0x13, 0xB5),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x5E),
                new ParameterDefinition(
                    Name: "eVL_SOLL_MAX_KUEHLEN_HZK0",
                    InfoNumber: new InfoNumber(0x13, 0x5E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x5D),
                new ParameterDefinition(
                    Name: "eVL_SOLL_START_KUEHLEN_HZK_0",
                    InfoNumber: new InfoNumber(0x13, 0x5D),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x2A),
                new ParameterDefinition(
                    Name: "eVORLAUFSOLLTEMP_NACHT",
                    InfoNumber: new InfoNumber(0x01, 0x2A),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x29),
                new ParameterDefinition(
                    Name: "eVORLAUFSOLLTEMP_TAG",
                    InfoNumber: new InfoNumber(0x01, 0x29),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 10,
                    Max: 70,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x9A),
                new ParameterDefinition(
                    Name: "eWP_AUSSENGERAET",
                    InfoNumber: new InfoNumber(0x06, 0x9A),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x70),
                new ParameterDefinition(
                    Name: "eWP_HT_NT_FKT_ANSCHLUSS",
                    InfoNumber: new InfoNumber(0x06, 0x70),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x6F),
                new ParameterDefinition(
                    Name: "eWP_HT_NT_FUNKTION",
                    InfoNumber: new InfoNumber(0x06, 0x6F),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x99),
                new ParameterDefinition(
                    Name: "eWP_INNENGERAET",
                    InfoNumber: new InfoNumber(0x06, 0x99),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x79),
                new ParameterDefinition(
                    Name: "eWP_INTERLINKFUNKTION",
                    InfoNumber: new InfoNumber(0x06, 0x79),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x94),
                new ParameterDefinition(
                    Name: "eWP_MODUS_SMART_GRID",
                    InfoNumber: new InfoNumber(0x06, 0x94),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x7E),
                new ParameterDefinition(
                    Name: "eWP_PWM_LEISTUNG_MAX",
                    InfoNumber: new InfoNumber(0x06, 0x7E),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x7F),
                new ParameterDefinition(
                    Name: "eWP_PWM_LEISTUNG_MIN",
                    InfoNumber: new InfoNumber(0x06, 0x7F),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x78),
                new ParameterDefinition(
                    Name: "eWP_RAUMTHERMOSTAT",
                    InfoNumber: new InfoNumber(0x06, 0x78),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x93),
                new ParameterDefinition(
                    Name: "eWP_SMART_GRID",
                    InfoNumber: new InfoNumber(0x06, 0x93),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0xFD, 0x4F),
                new ParameterDefinition(
                    Name: "eANTILEG_START_ZEIT",
                    InfoNumber: new InfoNumber(0xFD, 0x4F),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x05, 0x87),
                new ParameterDefinition(
                    Name: "eANTILEG_TEMP",
                    InfoNumber: new InfoNumber(0x05, 0x87),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x82),
                new ParameterDefinition(
                    Name: "eZIRKPUMPE_BEI_WWFREIGABE",
                    InfoNumber: new InfoNumber(0x01, 0x82),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x01, 0x3D),
                new ParameterDefinition(
                    Name: "eABWESEND_RAUMSOLLTEMP",
                    InfoNumber: new InfoNumber(0x01, 0x3D),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: 5,
                    Max: 40,
                    Default: 15,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x91),
                new ParameterDefinition(
                    Name: "eWP_HYSTERESE_DHW",
                    InfoNumber: new InfoNumber(0x06, 0x91),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x06, 0x92),
                new ParameterDefinition(
                    Name: "eWP_WARTEZEIT_BOH",
                    InfoNumber: new InfoNumber(0x06, 0x92),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x13, 0x55),
                new ParameterDefinition(
                    Name: "eFEIERTAGENDE_JAHR",
                    InfoNumber: new InfoNumber(0x13, 0x55),
                    Type: ParameterType.Int,
                    Factor: 1,
                    TransferThreshold: null,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: false,
                    WaterCircuit: false,
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x04),
                new ParameterDefinition(
                    Name: "eVORLAUFSOLLTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x04),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: true,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
            {
                new InfoNumber(0x00, 0x0F),
                new ParameterDefinition(
                    Name: "eVORLAUFISTTEMP",
                    InfoNumber: new InfoNumber(0x00, 0x0F),
                    Type: ParameterType.Float,
                    Factor: 10,
                    TransferThreshold: 0.5,
                    Writeable: false,
                    Min: null,
                    Max: null,
                    Default: null,
                    Display: true,
                    WaterCircuit: false,
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null
                )
            },
        };

    public static IReadOnlyDictionary<InfoNumber, ParameterDefinition> Parameters =>
        new Dictionary<InfoNumber, ParameterDefinition>(_parameters);

    public static void RegisterTestParameter(InfoNumber info, ParameterDefinition definition)
    {
        _parameters[info] = definition;
    }

    #endregion

    #region Subsystem Mapping

    /// <summary>
    /// Determines the subsystem type for a given parameter based on its InfoNumber.High byte.
    /// 
    /// Mapping rules:
    /// - 0x00–0x0F: Heat Generator subsystem
    /// - 0x14–0x1F: Heating Circuit subsystem  
    /// - 0x17–0x1B: Heating Circuit Module subsystem (overlaps with HC, specific ranges apply)
    /// </summary>
    public static DeviceType? GetSubsystemForParameter(InfoNumber infoNumber)
    {
        var high = infoNumber.High;

        // Heat Generator: 0x00–0x0F
        if (high is >= 0x00 and <= 0x0F)
            return DeviceType.HeatGenerator;

        // Heating Circuit Module: 0x17–0x1B (subset of HC)
        if (high is >= 0x17 and <= 0x1B)
            return DeviceType.HeatingCircuitModule;

        // Heating Circuit: 0x14–0x1F (broader range)
        if (high is >= 0x14 and <= 0x1F)
            return DeviceType.HeatingCircuit;

        return null;
    }

    #endregion

    #region Communication Profiles

    private static readonly IReadOnlyList<CanDevice> _heatGenerators =
        [
            CreateDevice("HG1", DeviceType.HeatGenerator, "0x69D", "0x31", "0x00", "0xFA", "0x69D", "0x30", "0x00", "0xFA", "0x180", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG2", DeviceType.HeatGenerator, "0x69D", "0x31", "0x01", "0xFA", "0x69D", "0x30", "0x01", "0xFA", "0x181", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG3", DeviceType.HeatGenerator, "0x69D", "0x31", "0x02", "0xFA", "0x69D", "0x30", "0x02", "0xFA", "0x182", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG4", DeviceType.HeatGenerator, "0x69D", "0x31", "0x03", "0xFA", "0x69D", "0x30", "0x03", "0xFA", "0x183", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG5", DeviceType.HeatGenerator, "0x69D", "0x31", "0x04", "0xFA", "0x69D", "0x30", "0x04", "0xFA", "0x184", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG6", DeviceType.HeatGenerator, "0x69D", "0x31", "0x05", "0xFA", "0x69D", "0x30", "0x05", "0xFA", "0x185", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG7", DeviceType.HeatGenerator, "0x69D", "0x31", "0x06", "0xFA", "0x69D", "0x30", "0x06", "0xFA", "0x186", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HG8", DeviceType.HeatGenerator, "0x69D", "0x31", "0x07", "0xFA", "0x69D", "0x30", "0x07", "0xFA", "0x187", "0xD2", "0x1D", "0xFA"),
        ];

    private static readonly IReadOnlyList<CanDevice> _heatingCircuits =
        [
            CreateDevice("HC1", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x00", "0xFA", "0x69D", "0x60", "0x00", "0xFA", "0x300", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC2", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x01", "0xFA", "0x69D", "0x60", "0x01", "0xFA", "0x301", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC3", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x02", "0xFA", "0x69D", "0x60", "0x02", "0xFA", "0x302", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC4", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x03", "0xFA", "0x69D", "0x60", "0x03", "0xFA", "0x303", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC5", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x04", "0xFA", "0x69D", "0x60", "0x04", "0xFA", "0x304", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC6", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x05", "0xFA", "0x69D", "0x60", "0x05", "0xFA", "0x305", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC7", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x06", "0xFA", "0x69D", "0x60", "0x06", "0xFA", "0x306", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC8", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x07", "0xFA", "0x69D", "0x60", "0x07", "0xFA", "0x307", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC9", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x08", "0xFA", "0x69D", "0x60", "0x08", "0xFA", "0x308", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC10", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x09", "0xFA", "0x69D", "0x60", "0x09", "0xFA", "0x309", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC11", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0A", "0xFA", "0x69D", "0x60", "0x0A", "0xFA", "0x30A", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC12", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0B", "0xFA", "0x69D", "0x60", "0x0B", "0xFA", "0x30B", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC13", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0C", "0xFA", "0x69D", "0x60", "0x0C", "0xFA", "0x30C", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC14", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0D", "0xFA", "0x69D", "0x60", "0x0D", "0xFA", "0x30D", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC15", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0E", "0xFA", "0x69D", "0x60", "0x0E", "0xFA", "0x30E", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HC16", DeviceType.HeatingCircuit, "0x69D", "0x61", "0x0F", "0xFA", "0x69D", "0x60", "0x0F", "0xFA", "0x30F", "0xD2", "0x1D", "0xFA"),
        ];

    private static readonly IReadOnlyList<CanDevice> _heatingCircuitModules =
        [
            CreateDevice("HCM1", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x00", "0xFA", "0x69D", "0xC0", "0x00", "0xFA", "0x600", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM2", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x01", "0xFA", "0x69D", "0xC0", "0x01", "0xFA", "0x601", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM3", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x02", "0xFA", "0x69D", "0xC0", "0x02", "0xFA", "0x602", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM4", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x03", "0xFA", "0x69D", "0xC0", "0x03", "0xFA", "0x603", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM5", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x04", "0xFA", "0x69D", "0xC0", "0x04", "0xFA", "0x604", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM6", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x05", "0xFA", "0x69D", "0xC0", "0x05", "0xFA", "0x605", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM7", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x06", "0xFA", "0x69D", "0xC0", "0x06", "0xFA", "0x606", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM8", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x07", "0xFA", "0x69D", "0xC0", "0x07", "0xFA", "0x607", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM9", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x08", "0xFA", "0x69D", "0xC0", "0x08", "0xFA", "0x608", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM10", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x09", "0xFA", "0x69D", "0xC0", "0x09", "0xFA", "0x609", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM11", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0A", "0xFA", "0x69D", "0xC0", "0x0A", "0xFA", "0x60A", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM12", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0B", "0xFA", "0x69D", "0xC0", "0x0B", "0xFA", "0x60B", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM13", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0C", "0xFA", "0x69D", "0xC0", "0x0C", "0xFA", "0x60C", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM14", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0D", "0xFA", "0x69D", "0xC0", "0x0D", "0xFA", "0x60D", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM15", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0E", "0xFA", "0x69D", "0xC0", "0x0E", "0xFA", "0x60E", "0xD2", "0x1D", "0xFA"),
            CreateDevice("HCM16", DeviceType.HeatingCircuitModule, "0x69D", "0xC1", "0x0F", "0xFA", "0x69D", "0xC0", "0x0F", "0xFA", "0x60F", "0xD2", "0x1D", "0xFA"),
        ];

    /// <summary>
    /// Gets all heat generators with their communication profiles
    /// </summary>
    public static IReadOnlyList<CanDevice> HeatGenerators => _heatGenerators;

    /// <summary>
    /// Gets all heating circuits with their communication profiles
    /// </summary>
    public static IReadOnlyList<CanDevice> HeatingCircuits => _heatingCircuits;

    /// <summary>
    /// Gets all heating circuit modules with their communication profiles
    /// </summary>
    public static IReadOnlyList<CanDevice> HeatingCircuitModules => _heatingCircuitModules;

    /// <summary>
    /// Gets a heat generator by name
    /// </summary>
    public static CanDevice? GetHeatGenerator(string name) =>
        _heatGenerators.FirstOrDefault(d => d.Name == name);

    /// <summary>
    /// Gets a heating circuit by name
    /// </summary>
    public static CanDevice? GetHeatingCircuit(string name) =>
        _heatingCircuits.FirstOrDefault(d => d.Name == name);

    /// <summary>
    /// Gets a heating circuit module by name
    /// </summary>
    public static CanDevice? GetHeatingCircuitModule(string name) =>
        _heatingCircuitModules.FirstOrDefault(d => d.Name == name);

    /// <summary>
    /// Gets a device by name, searching all device types
    /// </summary>
    public static CanDevice? GetDevice(string name)
    {
        return GetHeatGenerator(name)
            ?? GetHeatingCircuit(name)
            ?? GetHeatingCircuitModule(name);
    }

    private static CanDevice CreateDevice(
        string name,
        DeviceType type,
        string getCanId, string getByte0, string getByte1, string getByte2,
        string setCanId, string setByte0, string setByte1, string setByte2,
        string answerCanId, string answerByte0, string answerByte1, string answerByte2)
    {
        return new CanDevice
        {
            Name = name,
            Type = type,
            Profile = new CommunicationProfile
            {
                Name = name,
                Get = CommunicationCommand.Parse([getCanId, getByte0, getByte1, getByte2]),
                Set = CommunicationCommand.Parse([setCanId, setByte0, setByte1, setByte2]),
                Answer = CommunicationCommand.Parse([answerCanId, answerByte0, answerByte1, answerByte2])
            }
        };
    }

    #endregion
}