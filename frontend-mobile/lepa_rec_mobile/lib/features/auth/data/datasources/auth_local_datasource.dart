import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../models/auth_response.dart';

class AuthLocalDataSource {
  static const _accessTokenKey = 'access_token';
  static const _refreshTokenKey = 'refresh_token';
  static const _expiresAtKey = 'expires_at';
  static const _userIdKey = 'user_id';
  static const _roleKey = 'role';

  final FlutterSecureStorage _storage;

  AuthLocalDataSource({FlutterSecureStorage? storage})
      : _storage = storage ?? const FlutterSecureStorage();

  Future<void> saveSession(AuthResponse auth) async {
    await _storage.write(key: _accessTokenKey, value: auth.accessToken);
    await _storage.write(key: _refreshTokenKey, value: auth.refreshToken);
    await _storage.write(key: _expiresAtKey, value: auth.expiresAt.toIso8601String());
    await _storage.write(key: _userIdKey, value: auth.userId);
    await _storage.write(key: _roleKey, value: auth.role);
  }

  Future<String?> readAccessToken() => _storage.read(key: _accessTokenKey);
  Future<String?> readRefreshToken() => _storage.read(key: _refreshTokenKey);

  Future<void> clearSession() async {
    await _storage.delete(key: _accessTokenKey);
    await _storage.delete(key: _refreshTokenKey);
    await _storage.delete(key: _expiresAtKey);
    await _storage.delete(key: _userIdKey);
    await _storage.delete(key: _roleKey);
  }
}
