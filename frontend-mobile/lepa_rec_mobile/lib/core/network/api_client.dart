import 'package:dio/dio.dart';
import '../constants/app_constants.dart';
import 'auth_interceptor.dart';

class ApiClient {
  ApiClient._();

  static final Dio _dio = Dio(
    BaseOptions(
      baseUrl: AppConstants.apiBaseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
      headers: {'Content-Type': 'application/json'},
    ),
  );

   static Dio get dio => _dio;

  static void configure() {
    if (_dio.interceptors.whereType<AuthInterceptor>().isNotEmpty) return;
    _dio.interceptors.add(AuthInterceptor(_dio));
  }
}
