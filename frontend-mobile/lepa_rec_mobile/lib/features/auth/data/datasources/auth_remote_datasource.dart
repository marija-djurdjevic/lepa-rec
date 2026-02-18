import 'package:dio/dio.dart';
import '../../../../core/network/api_client.dart';
import '../models/auth_response.dart';
import '../models/google_login_request.dart';

class AuthRemoteDataSource {
  final Dio _dio;

  AuthRemoteDataSource({Dio? dio}) : _dio = dio ?? ApiClient.dio;

  Future<AuthResponse> googleLogin(String idToken) async {
    final request = GoogleLoginRequest(idToken: idToken);

    final response = await _dio.post(
      '/auth/google-login',
      data: request.toJson(),
    );

    return AuthResponse.fromJson(response.data as Map<String, dynamic>);
  }

  Future<AuthResponse> refresh(String refreshToken) async {
  final response = await _dio.post(
    '/auth/refresh',
    data: {'refreshToken': refreshToken},
  );

  return AuthResponse.fromJson(response.data as Map<String, dynamic>);
  }

}

