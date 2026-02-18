import 'dart:async';
import 'package:dio/dio.dart';
import '../../features/auth/data/datasources/auth_local_datasource.dart';
import '../../features/auth/data/datasources/auth_remote_datasource.dart';

class AuthInterceptor extends Interceptor {
  final Dio _dio;
  final AuthLocalDataSource _local;
  final AuthRemoteDataSource _remote;

  bool _isRefreshing = false;
  Completer<String?>? _refreshCompleter;

  AuthInterceptor(
    this._dio, {
    AuthLocalDataSource? local,
    AuthRemoteDataSource? remote,
  })  : _local = local ?? AuthLocalDataSource(),
        _remote = remote ?? AuthRemoteDataSource();

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await _local.readAccessToken();
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final statusCode = err.response?.statusCode;

    if (statusCode != 401) {
      handler.next(err);
      return;
    }

    if (err.requestOptions.path.contains('/auth/refresh')) {
      await _local.clearSession();
      handler.next(err);
      return;
    }

    final newAccessToken = await _refreshAccessToken();
    if (newAccessToken == null) {
      handler.next(err);
      return;
    }

    final requestOptions = err.requestOptions;

    requestOptions.headers['Authorization'] = 'Bearer $newAccessToken';

    try {
      final response = await _dio.fetch(requestOptions);
      handler.resolve(response);
    } catch (_) {
      handler.next(err);
    }
  }

  Future<String?> _refreshAccessToken() async {
    final refreshToken = await _local.readRefreshToken();
    if (refreshToken == null || refreshToken.isEmpty) {
      await _local.clearSession();
      return null;
    }

    if (_isRefreshing) {
      return _refreshCompleter?.future;
    }

    _isRefreshing = true;
    _refreshCompleter = Completer<String?>();

    try {
      final auth = await _remote.refresh(refreshToken);
      await _local.saveSession(auth);
      _refreshCompleter?.complete(auth.accessToken);
      return auth.accessToken;
    } catch (_) {
      await _local.clearSession();
      _refreshCompleter?.complete(null);
      return null;
    } finally {
      _isRefreshing = false;
    }
  }
}
