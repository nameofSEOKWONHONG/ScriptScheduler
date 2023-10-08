namespace ScriptScheduler.Domain.Enums;

public enum ENUM_REPEAT_TYPE
{
    /// <summary>
    /// 매일 한번 실행
    /// </summary>
    DAILY,
    /// <summary>
    /// 계속 실행
    /// </summary>
    CONTINUE,
    /// <summary>
    /// 한번만 실행 후 삭제
    /// </summary>
    ONCE,
}