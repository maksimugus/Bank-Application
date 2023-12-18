namespace Models.Accounts;

public record Account(long Id, long UserId, int Balance, int Pin);