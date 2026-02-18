import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';

import '../../data/datasources/auth_local_datasource.dart';
import '../../data/datasources/auth_remote_datasource.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _remote = AuthRemoteDataSource();
  final _local = AuthLocalDataSource();

  bool _loading = false;
  String? _error;

  Future<void> _signInWithGoogle() async {
    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      final googleSignIn = GoogleSignIn.instance;
      await googleSignIn.initialize();

      final account = await googleSignIn.authenticate();
      

      final authentication =  account.authentication;
      final idToken = authentication.idToken;

      if (idToken == null || idToken.isEmpty) {
        throw Exception('Google idToken is missing.');
      }

      final auth = await _remote.googleLogin(idToken);
      await _local.saveSession(auth);

      final saved = await _local.readAccessToken();
      debugPrint('SAVED ACCESS TOKEN? ${saved != null}');

      if (!mounted) return;
      Navigator.of(context).pushNamedAndRemoveUntil('/home', (_) => false);
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Login success ✅')),
      );
    } catch (e) {
      setState(() {
        _error = e.toString();
      });
    } finally {
      setState(() {
        _loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Login')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            if (_error != null) ...[
              Text(_error!, style: const TextStyle(color: Colors.red)),
              const SizedBox(height: 12),
            ],
            ElevatedButton(
              onPressed: _loading ? null : _signInWithGoogle,
              child: Text(_loading ? 'Loading...' : 'Continue with Google'),
            ),
          ],
        ),
      ),
    );
  }
}
