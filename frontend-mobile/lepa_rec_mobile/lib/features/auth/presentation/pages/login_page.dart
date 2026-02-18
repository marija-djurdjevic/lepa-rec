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

  final _phoneController = TextEditingController();

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

      final authentication = account.authentication;
      final idToken = authentication.idToken;

      if (idToken == null || idToken.isEmpty) {
        throw Exception('Google idToken is missing.');
      }

      final auth = await _remote.googleLogin(idToken);
      await _local.saveSession(auth);

      if (!mounted) return;

      Navigator.of(context).pushNamedAndRemoveUntil('/home', (_) => false);
    } catch (e) {
      setState(() {
        _error = 'Login failed. Please try again.';
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
      body: Stack(
        children: [
          _buildContent(),
          if (_loading) _buildLoadingOverlay(),
        ],
      ),
    );
  }

  Widget _buildContent() {
    return SafeArea(
      child: Center(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              _buildHeader(),
              const SizedBox(height: 40),

              _buildPhoneInput(),
              const SizedBox(height: 16),

              _buildPhoneButton(),
              const SizedBox(height: 24),

              _buildDivider(),
              const SizedBox(height: 24),

              _buildGoogleButton(),

              if (_error != null) ...[
                const SizedBox(height: 24),
                _buildErrorBox(),
              ],
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Column(
      children: const [
        Icon(Icons.favorite, size: 72, color: Colors.pink),
        SizedBox(height: 16),
        Text(
          'Lepa reč',
          style: TextStyle(
            fontSize: 32,
            fontWeight: FontWeight.bold,
          ),
        ),
        SizedBox(height: 8),
        Text(
          'Širi lepe reči svaki dan 💌',
          style: TextStyle(fontSize: 16, color: Colors.grey),
        ),
      ],
    );
  }

  Widget _buildPhoneInput() {
    return TextField(
      controller: _phoneController,
      keyboardType: TextInputType.phone,
      decoration: InputDecoration(
        labelText: 'Broj mobilnog',
        hintText: '+381 6X XXX XXXX',
        prefixIcon: const Icon(Icons.phone),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
        ),
      ),
    );
  }

  Widget _buildPhoneButton() {
    return SizedBox(
      height: 52,
      child: ElevatedButton(
        onPressed: () {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('SMS login uskoro dostupan 📱'),
            ),
          );
        },
        child: const Text('Nastavi putem broja telefona'),
      ),
    );
  }

  Widget _buildGoogleButton() {
    return SizedBox(
      height: 52,
      child: OutlinedButton.icon(
        onPressed: _loading ? null : _signInWithGoogle,
        icon: const Icon(Icons.login),
        label: const Text('Nastavi sa Google nalogom'),
      ),
    );
  }

  Widget _buildDivider() {
    return Row(
      children: const [
        Expanded(child: Divider()),
        Padding(
          padding: EdgeInsets.symmetric(horizontal: 12),
          child: Text('ili'),
        ),
        Expanded(child: Divider()),
      ],
    );
  }

  Widget _buildErrorBox() {
    return Container(
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: Colors.red.shade50,
        borderRadius: BorderRadius.circular(12),
      ),
      child: const Text(
        'Došlo je do greške prilikom prijave.',
        style: TextStyle(color: Colors.red),
      ),
    );
  }


  Widget _buildLoadingOverlay() {
    return Container(
      color: Colors.black.withOpacity(0.3),
      child: const Center(
        child: CircularProgressIndicator(),
      ),
    );
  }
}
