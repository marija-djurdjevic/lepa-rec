import 'package:flutter/material.dart';
import '../../features/auth/data/datasources/auth_local_datasource.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final local = AuthLocalDataSource();

    return Scaffold(
      appBar: AppBar(title: const Text('Home')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const Text('Ulogovana si ✅'),
            const SizedBox(height: 12),
            ElevatedButton(
              onPressed: () async {
                await local.clearSession();
                if (context.mounted) {
                  Navigator.of(context).pushNamedAndRemoveUntil('/login', (_) => false);
                }
              },
              child: const Text('Logout'),
            ),
          ],
        ),
      ),
    );
  }
}
