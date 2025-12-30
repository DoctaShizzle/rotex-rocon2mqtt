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
                    OriginalName: "cGERAETE_KENNUNG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "DeviceIdentifier"
                )
            },
            {
                new InfoNumber(0x00, 0x0C),
                new ParameterDefinition(
                    OriginalName: "cAUSSENTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "OutdoorTemperature"
                )
            },
            {
                new InfoNumber(0x0A, 0x0C),
                new ParameterDefinition(
                    OriginalName: "cAUSSENTEMP_WAERMEPUMPE",
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
                    DefaultTimeRange: null,
                    NameEnglish: "OutdoorTemperatureHeatPump"
                )
            },
            {
                new InfoNumber(0x01, 0x22),
                new ParameterDefinition(
                    OriginalName: "cTAG",
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
                    OriginalName: "cMONAT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "Month"
                )
            },
            {
                new InfoNumber(0x01, 0x24),
                new ParameterDefinition(
                    OriginalName: "cJAHR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "Day"
                )
            },
            {
                new InfoNumber(0x01, 0x25),
                new ParameterDefinition(
                    OriginalName: "cSTUNDE",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "Hour"
                )
            },
            {
                new InfoNumber(0x01, 0x26),
                new ParameterDefinition(
                    OriginalName: "cMINUTE",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "Minute"
                )
            },
            {
                new InfoNumber(0x01, 0x12),
                new ParameterDefinition(
                    OriginalName: "cPROGRAMMSCHALTER",
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
                    DefaultTimeRange: null,
                    NameEnglish: "ProgramSwitch"
                )
            },
            {
                new InfoNumber(0x00, 0x0E),
                new ParameterDefinition(
                    OriginalName: "cSPEICHERISTTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "BufferTemperatureActual"
                )
            },
            {
                new InfoNumber(0x00, 0x03),
                new ParameterDefinition(
                    OriginalName: "cVERSTELLTE_SPEICHERSOLLTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "BufferTemperatureSetpointAdjusted"
                )
            },
            {
                new InfoNumber(0x00, 0x13),
                new ParameterDefinition(
                    OriginalName: "cEINSTELL_SPEICHERSOLLTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "BufferTemperatureSetpoint"
                )
            },
            {
                new InfoNumber(0x0A, 0x06),
                new ParameterDefinition(
                    OriginalName: "cEINSTELL_SPEICHERSOLLTEMP2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "StorageTargetTemperatureSetting2"
                )
            },
            {
                new InfoNumber(0x01, 0x3E),
                new ParameterDefinition(
                    OriginalName: "cEINSTELL_SPEICHERSOLLTEMP3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "StorageActualTemperature"
                )
            },
            {
                new InfoNumber(0x17, 0x00),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1"
                )
            },
            {
                new InfoNumber(0x17, 0x10),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Monday"
                )
            },
            {
                new InfoNumber(0x17, 0x11),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1MondaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x12),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1MondaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x20),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DI",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Tuesday"
                )
            },
            {
                new InfoNumber(0x17, 0x21),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1TuesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x22),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1TuesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x30),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MI",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Wednesday"
                )
            },
            {
                new InfoNumber(0x17, 0x31),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1WednesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x32),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1WednesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x40),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DO",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Thursday"
                )
            },
            {
                new InfoNumber(0x17, 0x41),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1ThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x42),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_DO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1ThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x50),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_FR",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Friday"
                )
            },
            {
                new InfoNumber(0x17, 0x51),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_FR_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1FridaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x52),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_FR_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1FridaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x60),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Saturday"
                )
            },
            {
                new InfoNumber(0x17, 0x61),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1SaturdaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x62),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1SaturdaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x70),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SO",
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
                    DefaultTimeRange: "00:00-23:45",
                    NameEnglish: "WaterProgram1Sunday"
                )
            },
            {
                new InfoNumber(0x17, 0x71),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1SundaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x72),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram1SundaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x80),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_FR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToFriday"
                )
            },
            {
                new InfoNumber(0x17, 0x81),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_FR_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToFridaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x82),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_FR_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToFridaySwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0x90),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1Weekend"
                )
            },
            {
                new InfoNumber(0x17, 0x91),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1WeekendSwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0x92),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_SA_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1WeekendSwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0xA0),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1AllWeek"
                )
            },
            {
                new InfoNumber(0x17, 0xA1),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1AllWeekSwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0xA2),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1AllWeekSwitch3"
                )
            },
            {
                new InfoNumber(0x17, 0xB0),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_DO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToThursday"
                )
            },
            {
                new InfoNumber(0x17, 0xB1),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_DO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x17, 0xB2),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_1_MO_DO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram1MondayToThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x00),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2"
                )
            },
            {
                new InfoNumber(0x18, 0x10),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO",
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
                    DefaultTimeRange: "05:00-21:00",
                    NameEnglish: "WaterProgram2Monday"
                )
            },
            {
                new InfoNumber(0x18, 0x11),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2MondaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x12),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2MondaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x20),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DI",
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
                    DefaultTimeRange: "05:00-21:00",
                    NameEnglish: "WaterProgram2Tuesday"
                )
            },
            {
                new InfoNumber(0x18, 0x21),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2TuesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x22),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2TuesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x30),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MI",
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
                    DefaultTimeRange: "05:00-21:00",
                    NameEnglish: "WaterProgram2Wednesday"
                )
            },
            {
                new InfoNumber(0x18, 0x31),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2WednesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x32),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2WednesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x40),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DO",
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
                    DefaultTimeRange: "05:00-21:00",
                    NameEnglish: "WaterProgram2Thursday"
                )
            },
            {
                new InfoNumber(0x18, 0x41),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2ThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x42),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_DO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2ThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x50),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_FR",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2Friday"
                )
            },
            {
                new InfoNumber(0x18, 0x51),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_FR_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2FridaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x52),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_FR_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2FridaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x60),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA",
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
                    DefaultTimeRange: "06:00-22:00",
                    NameEnglish: "WaterProgram2Saturday"
                )
            },
            {
                new InfoNumber(0x18, 0x61),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2SaturdaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x62),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2SaturdaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x70),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SO",
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
                    DefaultTimeRange: "06:00-22:00",
                    NameEnglish: "WaterProgram2Sunday"
                )
            },
            {
                new InfoNumber(0x18, 0x71),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "StorageTargetTemperatureSetting3"
                )
            },
            {
                new InfoNumber(0x18, 0x72),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "WaterProgram2SundaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x80),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_FR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToFriday"
                )
            },
            {
                new InfoNumber(0x18, 0x81),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_FR_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToFridaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x82),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_FR_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToFridaySwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0x90),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2Weekend"
                )
            },
            {
                new InfoNumber(0x18, 0x91),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2WeekendSwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0x92),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_SA_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2WeekendSwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0xA0),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2AllWeek"
                )
            },
            {
                new InfoNumber(0x18, 0xA1),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2AllWeekSwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0xA2),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2AllWeekSwitch3"
                )
            },
            {
                new InfoNumber(0x18, 0xB0),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_DO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToThursday"
                )
            },
            {
                new InfoNumber(0x18, 0xB1),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_DO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x18, 0xB2),
                new ParameterDefinition(
                    OriginalName: "cW_WASSERPROG_2_MO_DO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2MondayToThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x01, 0x44),
                new ParameterDefinition(
                    OriginalName: "cEINMAL_WW_AKTIV",
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
                    DefaultTimeRange: null,
                    NameEnglish: "OneTimeHotWaterActive"
                )
            },
            {
                new InfoNumber(0x00, 0x5E),
                new ParameterDefinition(
                    OriginalName: "cWW_AKTIV",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HotWaterProductionActive"
                )
            },
            {
                new InfoNumber(0x00, 0x11),
                new ParameterDefinition(
                    OriginalName: "cRAUMISTTEMP",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "RoomTemperatureActual"
                )
            },
            {
                new InfoNumber(0x00, 0x12),
                new ParameterDefinition(
                    OriginalName: "cVERSTELLTE_RAUMSOLLTEMP",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "RoomTemperatureSetpointAdjusted"
                )
            },
            {
                new InfoNumber(0x00, 0x05),
                new ParameterDefinition(
                    OriginalName: "cRAUMSOLLTEMP_I",
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
                    DefaultTimeRange: null,
                    NameEnglish: "RoomTemperatureSetpoint"
                )
            },
            {
                new InfoNumber(0x00, 0x06),
                new ParameterDefinition(
                    OriginalName: "cRAUMSOLLTEMP_II",
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
                    DefaultTimeRange: null,
                    NameEnglish: "RoomTargetTemperature2"
                )
            },
            {
                new InfoNumber(0x00, 0x07),
                new ParameterDefinition(
                    OriginalName: "cRAUMSOLLTEMP_III",
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
                    DefaultTimeRange: null,
                    NameEnglish: "RoomTargetTemperature3"
                )
            },
            {
                new InfoNumber(0x00, 0x08),
                new ParameterDefinition(
                    OriginalName: "eNACHTRAUMSOLLTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "NightRoomTargetTemperature"
                )
            },
            {
                new InfoNumber(0x14, 0x00),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1"
                )
            },
            {
                new InfoNumber(0x14, 0x10),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO",
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
                    DefaultTimeRange: "06:00-22:00",
                    NameEnglish: "HeatingProgram1Monday"
                )
            },
            {
                new InfoNumber(0x14, 0x11),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1MondaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x12),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1MondaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x20),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DI",
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
                    DefaultTimeRange: "06:00-22:00",
                    NameEnglish: "HeatingProgram1Tuesday"
                )
            },
            {
                new InfoNumber(0x14, 0x21),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1TuesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x22),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1TuesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x30),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MI",
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
                    DefaultTimeRange: "06:00-22:00",
                    NameEnglish: "HeatingProgram1Wednesday"
                )
            },
            {
                new InfoNumber(0x14, 0x31),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1WednesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x32),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1WednesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x40),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DO",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1Thursday"
                )
            },
            {
                new InfoNumber(0x14, 0x41),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1ThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x42),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_DO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1ThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x50),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_FR",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1Friday"
                )
            },
            {
                new InfoNumber(0x14, 0x51),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_FR_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1FridaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x52),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_FR_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1FridaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x60),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA",
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
                    DefaultTimeRange: "07:00-23:00",
                    NameEnglish: "HeatingProgram1Saturday"
                )
            },
            {
                new InfoNumber(0x14, 0x61),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1SaturdaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x62),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1SaturdaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x70),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SO",
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
                    DefaultTimeRange: "07:00-23:00",
                    NameEnglish: "HeatingProgram1Sunday"
                )
            },
            {
                new InfoNumber(0x14, 0x71),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1SundaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x72),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram1SundaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x80),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_FR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToFriday"
                )
            },
            {
                new InfoNumber(0x14, 0x81),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_FR_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToFridaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x82),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_FR_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToFridaySwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0x90),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1Weekend"
                )
            },
            {
                new InfoNumber(0x14, 0x91),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1WeekendSwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0x92),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_SA_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1WeekendSwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0xA0),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1AllWeek"
                )
            },
            {
                new InfoNumber(0x14, 0xA1),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1AllWeekSwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0xA2),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1AllWeekSwitch3"
                )
            },
            {
                new InfoNumber(0x14, 0xB0),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_DO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToThursday"
                )
            },
            {
                new InfoNumber(0x14, 0xB1),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_DO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x14, 0xB2),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_1_MO_DO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram1MondayToThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x00),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterProgram2SundaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x10),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO",
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
                    DefaultTimeRange: "06:00-08:00",
                    NameEnglish: "HeatingProgram2Monday"
                )
            },
            {
                new InfoNumber(0x15, 0x11),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2MondaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x12),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2MondaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x20),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DI",
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
                    DefaultTimeRange: "06:00-08:00",
                    NameEnglish: "HeatingProgram2Tuesday"
                )
            },
            {
                new InfoNumber(0x15, 0x21),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2TuesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x22),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2TuesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x30),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MI",
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
                    DefaultTimeRange: "06:00-08:00",
                    NameEnglish: "HeatingProgram2Wednesday"
                )
            },
            {
                new InfoNumber(0x15, 0x31),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MI_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2WednesdaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x32),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MI_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2WednesdaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x40),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DO",
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
                    DefaultTimeRange: "06:00-08:00",
                    NameEnglish: "HeatingProgram2Thursday"
                )
            },
            {
                new InfoNumber(0x15, 0x41),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DO_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2ThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x42),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_DO_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2ThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x50),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_FR",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2Friday"
                )
            },
            {
                new InfoNumber(0x15, 0x51),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_FR_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2FridaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x52),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_FR_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2FridaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x60),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA",
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
                    DefaultTimeRange: "07:00-23:00",
                    NameEnglish: "HeatingProgram2Saturday"
                )
            },
            {
                new InfoNumber(0x15, 0x61),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA_SCHALT_2",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2SaturdaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x62),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA_SCHALT_3",
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
                    DefaultTimeRange: "00:00-00:00",
                    NameEnglish: "HeatingProgram2SaturdaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x70),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SO",
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
                    DefaultTimeRange: "07:00-23:00",
                    NameEnglish: "HeatingProgram2Sunday"
                )
            },
            {
                new InfoNumber(0x15, 0x71),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2SundaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x72),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2SundaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x80),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_FR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToFriday"
                )
            },
            {
                new InfoNumber(0x15, 0x81),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_FR_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToFridaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x82),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_FR_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToFridaySwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0x90),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2Weekend"
                )
            },
            {
                new InfoNumber(0x15, 0x91),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2WeekendSwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0x92),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_SA_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2WeekendSwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0xA0),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_SO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2AllWeek"
                )
            },
            {
                new InfoNumber(0x15, 0xA1),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_SO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2AllWeekSwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0xA2),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_SO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2AllWeekSwitch3"
                )
            },
            {
                new InfoNumber(0x15, 0xB0),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_DO",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToThursday"
                )
            },
            {
                new InfoNumber(0x15, 0xB1),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_DO_SCHALT_2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToThursdaySwitch2"
                )
            },
            {
                new InfoNumber(0x15, 0xB2),
                new ParameterDefinition(
                    OriginalName: "cHEIZPROG_2_MO_DO_SCHALT_3",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingProgram2MondayToThursdaySwitch3"
                )
            },
            {
                new InfoNumber(0x01, 0x99),
                new ParameterDefinition(
                    OriginalName: "cSOFTWARE_NUMMER",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SoftwareNumber"
                )
            },
            {
                new InfoNumber(0x01, 0x9A),
                new ParameterDefinition(
                    OriginalName: "cSOFTWARE_VERSION",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SoftwareVersion"
                )
            },
            {
                new InfoNumber(0x02, 0x4B),
                new ParameterDefinition(
                    OriginalName: "cSOFTWARE_UNTERINDEX",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SoftwareSubindex"
                )
            },
            {
                new InfoNumber(0x13, 0x58),
                new ParameterDefinition(
                    OriginalName: "cMODUS_PARTY_DAUER",
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
                    DefaultTimeRange: null,
                    NameEnglish: "PartyModeDuration"
                )
            },
            {
                new InfoNumber(0x01, 0x1B),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ANFANG_TAG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeStartDay"
                )
            },
            {
                new InfoNumber(0x01, 0x1C),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ANFANG_MONAT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeStartMonth"
                )
            },
            {
                new InfoNumber(0x01, 0x1D),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ANFANG_JAHR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeStartYear"
                )
            },
            {
                new InfoNumber(0x01, 0x1E),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ENDE_TAG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeEndDay"
                )
            },
            {
                new InfoNumber(0x01, 0x1F),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ENDE_MONAT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeEndMonth"
                )
            },
            {
                new InfoNumber(0x01, 0x20),
                new ParameterDefinition(
                    OriginalName: "cMODUS_URLAUB_ENDE_JAHR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "VacationModeEndYear"
                )
            },
            {
                new InfoNumber(0x00, 0x0D),
                new ParameterDefinition(
                    OriginalName: "cKESSELISTTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeaterTemperatureActual"
                )
            },
            {
                new InfoNumber(0x00, 0x02),
                new ParameterDefinition(
                    OriginalName: "cKESSELSOLLTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeaterTemperatureSetpoint"
                )
            },
            {
                new InfoNumber(0x00, 0x16),
                new ParameterDefinition(
                    OriginalName: "cRUECKLAUFTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "TR"
                )
            },
            {
                new InfoNumber(0x01, 0xDA),
                new ParameterDefinition(
                    OriginalName: "cVOLUMENSTROM",
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
                    DefaultTimeRange: null,
                    NameEnglish: "V"
                )
            },
            {
                new InfoNumber(0xC0, 0xF7),
                new ParameterDefinition(
                    OriginalName: "cPWM_SIGNAL",
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
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "n"
                )
            },
            {
                new InfoNumber(0x06, 0x9B),
                new ParameterDefinition(
                    OriginalName: "cMISCHERSTELLUNG_1_3UV1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "ValveCH_DHW"
                )
            },
            {
                new InfoNumber(0xC0, 0xFB),
                new ParameterDefinition(
                    OriginalName: "cMISCHERSTELLUNG_2_3UVB",
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
                    DefaultTimeRange: null,
                    NameEnglish: "ValveCH_Bypass"
                )
            },
            {
                new InfoNumber(0xC1, 0x02),
                new ParameterDefinition(
                    OriginalName: "cT_TVBH1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "TVBH1"
                )
            },
            {
                new InfoNumber(0xC0, 0xFE),
                new ParameterDefinition(
                    OriginalName: "cT_TVBHMIX",
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
                    DefaultTimeRange: null,
                    NameEnglish: "TVBHMIX"
                )
            },
            {
                new InfoNumber(0xC1, 0xBF),
                new ParameterDefinition(
                    OriginalName: "cT_TVBH",
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
                    DefaultTimeRange: null,
                    NameEnglish: "TVBH"
                )
            },
            {
                new InfoNumber(0xC0, 0xF6),
                new ParameterDefinition(
                    OriginalName: "cDEFROST_AKTIV",
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
                    BigEndian: true,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "Defrost   "
                )
            },
            {
                new InfoNumber(0x06, 0xA4),
                new ParameterDefinition(
                    OriginalName: "cPUMPENLAUFZEIT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "PumpOperatingHours"
                )
            },
            {
                new InfoNumber(0x06, 0xA5),
                new ParameterDefinition(
                    OriginalName: "cKOMPRESSORLAUFZEIT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "CompressorOperatingHours"
                )
            },
            {
                new InfoNumber(0x06, 0x61),
                new ParameterDefinition(
                    OriginalName: "cVMIN_A1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MinimumVolumeFlowA1"
                )
            },
            {
                new InfoNumber(0x06, 0x88),
                new ParameterDefinition(
                    OriginalName: "cVMIN_WP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MinimumVolumeFlowHeatPump"
                )
            },
            {
                new InfoNumber(0xC0, 0x9D),
                new ParameterDefinition(
                    OriginalName: "cVMIN_GCU",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MinimumVolumeFlowGCU"
                )
            },
            {
                new InfoNumber(0x13, 0x88),
                new ParameterDefinition(
                    OriginalName: "cFEHLER_AKTUELL",
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
                    DefaultTimeRange: null,
                    NameEnglish: "CurrentError"
                )
            },
            {
                new InfoNumber(0x0A, 0x1F),
                new ParameterDefinition(
                    OriginalName: "eZEITMASTER",
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
                    DefaultTimeRange: null,
                    NameEnglish: "TimeMaster"
                )
            },
            {
                new InfoNumber(0xC0, 0xB3),
                new ParameterDefinition(
                    OriginalName: "eSCHALTSCHWELLE_TDHW",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SwitchingThresholdDHW"
                )
            },
            {
                new InfoNumber(0xC0, 0xB1),
                new ParameterDefinition(
                    OriginalName: "eSONDERFKT_SCHALTKONTAKT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SpecialFunctionSwitchContact"
                )
            },
            {
                new InfoNumber(0xC0, 0xB2),
                new ParameterDefinition(
                    OriginalName: "eWARTEZEIT_SONDERFKT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SpecialFunctionWaitTime"
                )
            },
            {
                new InfoNumber(0x07, 0x26),
                new ParameterDefinition(
                    OriginalName: "eWASSER_MAX_DRUCKVERLUST",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterMaxPressureLoss"
                )
            },
            {
                new InfoNumber(0x07, 0x27),
                new ParameterDefinition(
                    OriginalName: "eWASSER_MAXIMALDRUCK",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterMaximumPressure"
                )
            },
            {
                new InfoNumber(0x07, 0x28),
                new ParameterDefinition(
                    OriginalName: "eWASSER_MINIMALDRUCK",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterMinimumPressure"
                )
            },
            {
                new InfoNumber(0x07, 0x25),
                new ParameterDefinition(
                    OriginalName: "eWASSER_SOLLDRUCK",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WaterTargetPressure"
                )
            },
            {
                new InfoNumber(0x06, 0x96),
                new ParameterDefinition(
                    OriginalName: "eWP_FLUESTERBETRIEB",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpQuietMode"
                )
            },
            {
                new InfoNumber(0x06, 0x69),
                new ParameterDefinition(
                    OriginalName: "eWP_LEISTUNG_HEIZSTAB_S1",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpHeatingElementPowerS1"
                )
            },
            {
                new InfoNumber(0x06, 0x6A),
                new ParameterDefinition(
                    OriginalName: "eWP_LEISTUNG_HEIZSTAB_S2",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpHeatingElementPowerS2"
                )
            },
            {
                new InfoNumber(0x06, 0x6B),
                new ParameterDefinition(
                    OriginalName: "eWP_LEISTUNG_HZU_BIV",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpAuxiliaryHeatingPowerBivalent"
                )
            },
            {
                new InfoNumber(0x06, 0x6E),
                new ParameterDefinition(
                    OriginalName: "eWP_MAX_TEMP_HEIZUNG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpMaxHeatingTemperature"
                )
            },
            {
                new InfoNumber(0x06, 0x82),
                new ParameterDefinition(
                    OriginalName: "eWP_MOD_HYST_DURCHFLUSS",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpModulationHysteresisFlow"
                )
            },
            {
                new InfoNumber(0x06, 0xA0),
                new ParameterDefinition(
                    OriginalName: "eWP_SOLLWERT_ANPASSUNG_HEIZEN",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSetpointAdjustmentHeating"
                )
            },
            {
                new InfoNumber(0x06, 0xA1),
                new ParameterDefinition(
                    OriginalName: "eWP_SOLLWERT_ANPASSUNG_KUEHLEN",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSetpointAdjustmentCooling"
                )
            },
            {
                new InfoNumber(0x06, 0x83),
                new ParameterDefinition(
                    OriginalName: "eWP_SPREIZUNG_HZ_BETRIEB",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSpreadHeatingOperation"
                )
            },
            {
                new InfoNumber(0x06, 0x84),
                new ParameterDefinition(
                    OriginalName: "eWP_SPREIZUNG_WW_BETRIEB",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSpreadHotWaterOperation"
                )
            },
            {
                new InfoNumber(0x06, 0x8C),
                new ParameterDefinition(
                    OriginalName: "eWP_START_MAX_TEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpStartMaxTemperature"
                )
            },
            {
                new InfoNumber(0x06, 0x85),
                new ParameterDefinition(
                    OriginalName: "eWP_VERZ_ZEIT_PUMPE",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpPumpDelayTime"
                )
            },
            {
                new InfoNumber(0x01, 0x2E),
                new ParameterDefinition(
                    OriginalName: "eABSENKOPTIMIERUNG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "LoweringOptimization"
                )
            },
            {
                new InfoNumber(0x01, 0x15),
                new ParameterDefinition(
                    OriginalName: "eADAPTION",
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
                    DefaultTimeRange: null,
                    NameEnglish: "Adaptation"
                )
            },
            {
                new InfoNumber(0x01, 0x03),
                new ParameterDefinition(
                    OriginalName: "eAUFHEIZOPTIMIERUNG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingUpOptimization"
                )
            },
            {
                new InfoNumber(0x0A, 0x00),
                new ParameterDefinition(
                    OriginalName: "eFROSTSCHUTZTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "FrostProtectionTemperature"
                )
            },
            {
                new InfoNumber(0x01, 0x17),
                new ParameterDefinition(
                    OriginalName: "eHEIZGRENZE_NACHT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingLimitNight"
                )
            },
            {
                new InfoNumber(0x01, 0x16),
                new ParameterDefinition(
                    OriginalName: "eHEIZGRENZE_TAG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingLimitDay"
                )
            },
            {
                new InfoNumber(0x01, 0x41),
                new ParameterDefinition(
                    OriginalName: "eHZK_FUNKTION",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatingCircuitFunction"
                )
            },
            {
                new InfoNumber(0x01, 0x0E),
                new ParameterDefinition(
                    OriginalName: "eHZKKURVE",
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
                    DefaultTimeRange: null,
                    NameEnglish: "WeatherDependentCurveSetting"
                )
            },
            {
                new InfoNumber(0x13, 0x59),
                new ParameterDefinition(
                    OriginalName: "eKUEHLSOLLWERT_KORR_HZK_0",
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
                    DefaultTimeRange: null,
                    NameEnglish: "CoolingSetpointCorrectionHeatingCircuit0"
                )
            },
            {
                new InfoNumber(0x01, 0x10),
                new ParameterDefinition(
                    OriginalName: "eMAX_AUFHEIZVORVERLEGUNG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MaxHeatingUpAdvance"
                )
            },
            {
                new InfoNumber(0x13, 0x5C),
                new ParameterDefinition(
                    OriginalName: "eMAX_KUEHLEN_AUSSENTEMP_HZK0",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MaxCoolingOutdoorTempHeatingCircuit0"
                )
            },
            {
                new InfoNumber(0x00, 0x28),
                new ParameterDefinition(
                    OriginalName: "eMAX_VORLAUFTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MaxSupplyTemperature"
                )
            },
            {
                new InfoNumber(0x01, 0x2B),
                new ParameterDefinition(
                    OriginalName: "eMIN_VORLAUFTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "MinSupplyTemperature"
                )
            },
            {
                new InfoNumber(0x01, 0x0F),
                new ParameterDefinition(
                    OriginalName: "eRAUMEINFLUSS",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "RoomInfluence"
                )
            },
            {
                new InfoNumber(0x13, 0xB5),
                new ParameterDefinition(
                    OriginalName: "eSTART_KUEHLEN_AUSSENTEMP_HZK0",
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
                    DefaultTimeRange: null,
                    NameEnglish: "StartCoolingOutdoorTempHeatingCircuit0"
                )
            },
            {
                new InfoNumber(0x13, 0x5E),
                new ParameterDefinition(
                    OriginalName: "eVL_SOLL_MAX_KUEHLEN_HZK0",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SupplyTargetMaxCoolingHeatingCircuit0"
                )
            },
            {
                new InfoNumber(0x13, 0x5D),
                new ParameterDefinition(
                    OriginalName: "eVL_SOLL_START_KUEHLEN_HZK_0",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SupplyTargetStartCoolingHeatingCircuit0"
                )
            },
            {
                new InfoNumber(0x01, 0x2A),
                new ParameterDefinition(
                    OriginalName: "eVORLAUFSOLLTEMP_NACHT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SupplyTargetTemperatureNight"
                )
            },
            {
                new InfoNumber(0x01, 0x29),
                new ParameterDefinition(
                    OriginalName: "eVORLAUFSOLLTEMP_TAG",
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
                    DefaultTimeRange: null,
                    NameEnglish: "SupplyTargetTemperatureDay"
                )
            },
            {
                new InfoNumber(0x06, 0x9A),
                new ParameterDefinition(
                    OriginalName: "eWP_AUSSENGERAET",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpOutdoorUnit"
                )
            },
            {
                new InfoNumber(0x06, 0x70),
                new ParameterDefinition(
                    OriginalName: "eWP_HT_NT_FKT_ANSCHLUSS",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpHighLowTariffFunctionConnection"
                )
            },
            {
                new InfoNumber(0x06, 0x6F),
                new ParameterDefinition(
                    OriginalName: "eWP_HT_NT_FUNKTION",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpHighLowTariffFunction"
                )
            },
            {
                new InfoNumber(0x06, 0x99),
                new ParameterDefinition(
                    OriginalName: "eWP_INNENGERAET",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpIndoorUnit"
                )
            },
            {
                new InfoNumber(0x06, 0x79),
                new ParameterDefinition(
                    OriginalName: "eWP_INTERLINKFUNKTION",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpInterlinkFunction"
                )
            },
            {
                new InfoNumber(0x06, 0x94),
                new ParameterDefinition(
                    OriginalName: "eWP_MODUS_SMART_GRID",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSmartGridMode"
                )
            },
            {
                new InfoNumber(0x06, 0x7E),
                new ParameterDefinition(
                    OriginalName: "eWP_PWM_LEISTUNG_MAX",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpPWMPowerMax"
                )
            },
            {
                new InfoNumber(0x06, 0x7F),
                new ParameterDefinition(
                    OriginalName: "eWP_PWM_LEISTUNG_MIN",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpPWMPowerMin"
                )
            },
            {
                new InfoNumber(0x06, 0x78),
                new ParameterDefinition(
                    OriginalName: "eWP_RAUMTHERMOSTAT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpRoomThermostat"
                )
            },
            {
                new InfoNumber(0x06, 0x93),
                new ParameterDefinition(
                    OriginalName: "eWP_SMART_GRID",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpSmartGrid"
                )
            },
            {
                new InfoNumber(0xFD, 0x4F),
                new ParameterDefinition(
                    OriginalName: "eANTILEG_START_ZEIT",
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
                    DefaultTimeRange: null,
                    NameEnglish: "AntiLegionellaStartTime"
                )
            },
            {
                new InfoNumber(0x05, 0x87),
                new ParameterDefinition(
                    OriginalName: "eANTILEG_TEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "AntiLegionellaTemperature"
                )
            },
            {
                new InfoNumber(0x01, 0x82),
                new ParameterDefinition(
                    OriginalName: "eZIRKPUMPE_BEI_WWFREIGABE",
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
                    DefaultTimeRange: null,
                    NameEnglish: "CirculationPumpAtHotWaterRelease"
                )
            },
            {
                new InfoNumber(0x01, 0x3D),
                new ParameterDefinition(
                    OriginalName: "eABWESEND_RAUMSOLLTEMP",
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
                    DefaultTimeRange: null,
                    NameEnglish: "AwayRoomTargetTemperature"
                )
            },
            {
                new InfoNumber(0x06, 0x91),
                new ParameterDefinition(
                    OriginalName: "eWP_HYSTERESE_DHW",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpHysteresisDHW"
                )
            },
            {
                new InfoNumber(0x06, 0x92),
                new ParameterDefinition(
                    OriginalName: "eWP_WARTEZEIT_BOH",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HeatPumpWaitTimeBOH"
                )
            },
            {
                new InfoNumber(0x13, 0x55),
                new ParameterDefinition(
                    OriginalName: "eFEIERTAGENDE_JAHR",
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
                    DefaultTimeRange: null,
                    NameEnglish: "HolidayEndYear"
                )
            },
            {
                new InfoNumber(0x00, 0x04),
                new ParameterDefinition(
                    OriginalName: "eVORLAUFSOLLTEMP",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "LeavingWaterTemperatureSetpoint"
                )
            },
            {
                new InfoNumber(0x00, 0x0F),
                new ParameterDefinition(
                    OriginalName: "eVORLAUFISTTEMP",
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
                    BigEndian: false,
                    EnumValues: null,
                    DefaultTimeRange: null,
                    NameEnglish: "LeavingWaterTemperatureActual"
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