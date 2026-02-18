import 'auth_local_datasource.dart';

class AuthSessionReader {
  final AuthLocalDataSource _local;

  AuthSessionReader({AuthLocalDataSource? local})
      : _local = local ?? AuthLocalDataSource();

  Future<bool> hasSession() async {
    final refresh = await _local.readRefreshToken();
    return refresh != null && refresh.isNotEmpty;
  }
}
