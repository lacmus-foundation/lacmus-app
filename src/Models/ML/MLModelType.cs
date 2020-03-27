namespace LacmusApp.Models.ML
{
    public enum MLModelType
    {
        //description                                                          :tag pattern
        Cpu         = 0,  // Cpu inference                                     :{api}.{ver}-cpu
        CpuNoAvx    = 1,  // Cpu inference (without avx)                       :{api}.{ver}-cpu-no-avx
        Gpu         = 10, // Gpu inference                                     :{api}.{ver}-gpu
        Tpu         = 20, // full edge tpu inference                           :{api}.{ver}-tpu
        TpuNoAvx    = 21, // full edge tpu inference (without avx)             :{api}.{ver}-tpu-no-avx
        TpuCpu      = 22, // edge tpu preprocess + cpu inference               :{api}.{ver}-tpu-cpu
        TpuCpuNoAvx = 23, // edge tpu preprocess + cpu inference (without awx) :{api}.{ver}-tpu-cpu-no-avx
        TpuGpu      = 24, // edge tpu preprocess + gpu inference               :{api}.{ver}-gpu
    }
}