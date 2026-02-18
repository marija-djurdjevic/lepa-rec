class AuthResponse {
  final String accessToken;
  final String refreshToken;
  final DateTime expiresAt;
  final String userId;
  final String role;

  AuthResponse({
    required this.accessToken,
    required this.refreshToken,
    required this.expiresAt,
    required this.userId,
    required this.role,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) {
    return AuthResponse(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      expiresAt: DateTime.parse(json['expiresAt'] as String),
      userId: json['userId'] as String,
      role: (json['role'] as String?) ?? '',
    );
  }
}
